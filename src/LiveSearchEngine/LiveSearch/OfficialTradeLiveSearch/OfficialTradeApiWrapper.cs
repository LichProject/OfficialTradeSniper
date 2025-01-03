namespace LiveSearchEngine.LiveSearch.OfficialTradeLiveSearch
{
    /// <summary>
    /// Official trade site API wrapper.
    /// </summary>
    public class OfficialTradeApiWrapper
    {
        private static string userAgent;
        private readonly string officialTradeApiUrl;
        private readonly string officialTradeUrl;

        public const int MaxItemsPerFetch = 10;
        public const int MaxExchangePerFetch = 20;

        public OfficialTradeApiWrapper(OfficialTradeConfiguration configuration)
        {
            UseNewConfiguration(configuration);

            officialTradeApiUrl = configuration.UsePoe2Api
                ? OfficialTradeConstants.OfficialTradeApiUrlPoe2
                : OfficialTradeConstants.OfficialTradeApiUrl;

            officialTradeUrl = configuration.UsePoe2Api
                ? OfficialTradeConstants.OfficialTradeUrlPoe2
                : OfficialTradeConstants.OfficialTradeUrl;

            userAgent = configuration.UserAgent;
            _restClient = new(
                officialTradeApiUrl,
                options => options.UserAgent = userAgent ?? RandomUa.RandomUserAgent,
                configureSerialization: cfg => cfg.UseNewtonsoftJson());
        }

        public AsyncPolicyWrap RateLimitPolicy { get; private set; }

        public void UseNewConfiguration(OfficialTradeConfiguration configuration) =>
            _configuration = configuration;

        private RestRequest CreateRequest(string uri, Method method)
        {
            RestRequest request = new(uri, method);

            request.AddCookie(OfficialTradeConstants.PoeSessionIdCookieName, _configuration.PoeSessionId, "/", ".pathofexile.com");
            request.AddHeader("Host", OfficialTradeConstants.OfficialSiteDomain);
            request.AddHeader("X-Requested-With", "XMLHttpRequest");

            return request;
        }

        public async Task<bool> SendWhisperRegularAsync(string token)
        {
            RestRequest request = CreateRequest($"{officialTradeApiUrl}/whisper", Method.Post);

            request.AddJsonBody(new { token });

            RestResponse<WhisperResponse> response = await _restClient.ExecuteAsync<WhisperResponse>(request);
            ThrowIfRequestFailed(response);

            return response.Data.Success;
        }

        public async Task<bool> SendWhisperExchangeAsync(string token, int multiplier)
        {
            RestRequest request = CreateRequest($"{officialTradeApiUrl}/whisper", Method.Post);

            request.AddJsonBody(new { token, values = new[] { multiplier } });

            RestResponse<WhisperResponse> response = await _restClient.ExecuteAsync<WhisperResponse>(request);
            return response.IsSuccessful && response.Data.Success;
        }

        public async Task<SearchResponse> SearchResultsByQueryAsync(object query, string league)
        {
            RestRequest request = CreateRequest($"{officialTradeApiUrl}/search{(_configuration.UsePoe2Api ? "/poe2" : "")}/{league}", Method.Post);

            request.AddJsonBody(query);

            RestResponse<SearchResponse> response = await GetRequestAsync<SearchResponse>(request);
            ThrowIfRequestFailed(response);

            return response.Data;
        }

        private async Task<SearchResponse> SearchResultsAsync(string league, string queryId, bool exchange)
        {
            string section = exchange
                ? "exchange"
                : "search";

            RestRequest request = CreateRequest($"{officialTradeUrl}/{section}/{league}/{queryId}", Method.Get);

            RestResponse response = await _restClient.ExecuteAsync(request);
            ThrowIfRequestFailed(response);

            Match match = Regex.Match(response.Content!, @"""state"":(.+)\n}(?:\);})", RegexOptions.Singleline);
            if (!match.Success)
            {
                throw new InvalidOperationException("Html content parsing error. Status: " + response.StatusCode);
            }

            string htmlQuery = match.Groups[1].Value;

            request = CreateRequest($"{officialTradeApiUrl}/{section}/{league}", Method.Post);

            if (exchange)
            {
                ExchangeSectionFilterHtml query = JsonConvert.DeserializeObject<ExchangeSectionFilterHtml>(htmlQuery);
                ExchangeHtml queryExchange = query.Exchange;

                string have = queryExchange.Have.Keys.FirstOrDefault();
                string want = queryExchange.Want.Keys.FirstOrDefault();
                int min = queryExchange.Stock?.Min ?? 1;

                return await SearchExchangeAsync(league, have, want, min);
            }
            else
            {
                IDictionary<string, object> query = JsonConvert.DeserializeObject<ExpandoObject>(htmlQuery);
                query.Remove("disc");
                query["status"] = new { option = "onlineleague" };
                var sort = new { price = "asc" };
                request.AddJsonBody(new { query, sort });
            }

            RestResponse<SearchResponse> result = await GetRequestAsync<SearchResponse>(request);
            ThrowIfRequestFailed(result);

            return result.Data;
        }

        public async Task<SearchResponseExchange> SearchExchangeByQueryIdAsync(string league, string queryId) =>
            (SearchResponseExchange)(await SearchResultsAsync(league, queryId, true));

        public Task<SearchResponse> SearchByQueryIdAsync(string league, ref string queryId) =>
            SearchResultsAsync(league, queryId, false);

        public async Task<SearchResponseExchange> SearchExchangeAsync(
            string league,
            string have,
            string want,
            int min = 1,
            int delay = -1)
        {
            RestRequest request = CreateRequest($"{officialTradeApiUrl}/exchange/{league}", Method.Post);

            ExchangeRequest exchangeRequest = new ExchangeRequest
            {
                Engine = "new",
                Query = new Exchange
                {
                    Have = have != null ? new[] { have } : Array.Empty<string>(),
                    Want = want != null ? new[] { want } : Array.Empty<string>(),
                    Stock = new ExchangeRequestStock { Min = min },
                    Status = new Status { Option = "online" }
                },
                Sort = new ExchangeSort { Have = "asc" }
            };

            request.AddJsonBody(exchangeRequest);

            RestResponse<SearchResponseExchange> response = await GetRequestAsync<SearchResponseExchange>(request, delay);
            ThrowIfRequestFailed(response);

            return response.Data;
        }

        /// <inheritdoc cref="Fetch(string[],string)"/>
        public async Task<FetchResponse> FetchAsync(
            IEnumerable<string> csvHashes,
            string queryId,
            int limit,
            bool exchange = false)
        {
            limit = Math.Min(
                exchange
                    ? MaxExchangePerFetch
                    : MaxItemsPerFetch,
                limit);

            return await FetchAsync(csvHashes.Take(limit).ToArray(), queryId, exchange);
        }

        public async Task<FetchResponse> FetchAsync(string[] csvHashes, string queryId, bool exchange = false)
        {
            RestRequest request = CreateRequest("/fetch/" + string.Join(",", csvHashes), Method.Get);
            request.AddQueryParameter("query", queryId);

            if (exchange)
            {
                request.AddQueryParameter("exchange", "1");
            }

            RestResponse<FetchResponse> response = await GetRequestAsync<FetchResponse>(request);
            ThrowIfRequestFailed(response);

            return response.Data;
        }

        private async Task<RestResponse<T>> GetRequestAsync<T>(RestRequest request, int overridenDelay = -1) where T : class
        {
            RestResponse<T> response = null;

            if (RateLimitPolicy != null)
            {
                await RateLimitPolicy.ExecuteAsync(async () => response = await _restClient.ExecuteAsync<T>(request));
            }
            else
            {
                response = await _restClient.ExecuteAsync<T>(request);

                AdjustRateLimit(response);
                AdjustUserAgent(response);
            }

            return response;
        }

        private void AdjustRateLimit(RestResponse response)
        {
            Parameter ipLimit = response.Headers?.FirstOrDefault(x => x.Name == "X-Rate-Limit-Ip");
            Parameter accountLimit = response.Headers?.FirstOrDefault(x => x.Name == "X-Rate-Limit-Account");

            if (ipLimit != null || accountLimit != null)
            {
                List<RateLimit> accountRate = ParseHeader(accountLimit?.Value as string).ToList();
                List<RateLimit> ipRate = ParseHeader(ipLimit?.Value as string).ToList();

                AsyncRetryPolicy retryPolicy = Policy
                    .Handle<RateLimitRejectedException>()
                    .RetryForeverAsync();

                IAsyncPolicy[] rateLimits = accountRate.Concat(ipRate)
                    .Select(rate => Polly.RateLimit.RateLimitPolicy.RateLimitAsync(rate.TotalRequests, TimeSpan.FromSeconds(rate.TotalSeconds), rate.TotalRequests / 2))
                    .ToArray<IAsyncPolicy>();

                AsyncPolicyWrap rateLimitPolicy = Policy.WrapAsync(rateLimits);

                RateLimitPolicy = retryPolicy.WrapAsync(rateLimitPolicy);
            }
        }

        private void AdjustUserAgent(RestResponse response)
        {
            if (response.StatusCode == HttpStatusCode.OK && userAgent == null)
            {
                userAgent = _restClient.DefaultParameters
                    .GetParameters(ParameterType.HttpHeader)
                    .FirstOrDefault(x => x.Name == "User-Agent")
                    ?.Value?.ToString();
            }
        }

        private static void ThrowIfRequestFailed(RestResponse response)
        {
            if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(response.Content))
            {
                return;
            }

            string reason = "Request failed. StatusCode: " + response.StatusCode;

            if (!string.IsNullOrEmpty(response.Content) && response.Content.Contains(@"""error"""))
            {
                reason += "\n" + response.Content;
            }

            throw new HttpRequestException(reason);
        }

        private static IEnumerable<RateLimit> ParseHeader(string header)
        {
            if (header == null)
            {
                yield break;
            }

            foreach (RateLimit rateLimit in header
                         .Split(',')
                         .Select(
                             part =>
                             {
                                 string[] segments = part.Split(':');
                                 return new RateLimit
                                 {
                                     TotalRequests = int.Parse(segments[0]),
                                     TotalSeconds = int.Parse(segments[1])
                                 };
                             }))
            {
                yield return rateLimit;
            }
        }

        private OfficialTradeConfiguration _configuration;
        private readonly RestClient _restClient;
    }
}