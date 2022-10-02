using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using LiveSearchEngine.Interfaces;
using LiveSearchEngine.Models.Poe;
using LiveSearchEngine.Models.Poe.Fetch;
using LiveSearchEngine.Models.Poe.Search;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RandomUserAgent;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace LiveSearchEngine.LiveSearch.OfficialTradeLiveSearch
{
    /// <summary>
    /// Official trade site API wrapper.
    /// </summary>
    public class OfficialTradeApiWrapper
    {
        static string _userAgent;

        public const int MaxItemsPerFetch = 10;
        public const int MaxExchangePerFetch = 20;

        public OfficialTradeApiWrapper(OfficialTradeConfiguration configuration,
                                       IRateLimit rateLimit)
        {
            UseNewConfiguration(configuration);

            RateLimit = rateLimit;

            _restClient = new RestClient(OfficialTradeConstants.OfficialTradeApiUrl);
            _restClient.UseNewtonsoftJson();

            _userAgent = configuration.UserAgent;
        }

        public IRateLimit RateLimit { get; }

        public void UseNewConfiguration(OfficialTradeConfiguration configuration)
        {
            _configuration = configuration;
        }

        RestRequest CreateRequest(string uri, Method method)
        {
            var request = new RestRequest(uri, method);
            request.AddCookie(
                OfficialTradeConstants.PoeSessionIdCookieName,
                _configuration.PoeSessionId);
            
            _restClient.UserAgent = _userAgent ?? RandomUa.RandomUserAgent;

            return request;
        }

        public bool SendWhisper(string token, int mySaleAmount)
        {
            var request = CreateRequest(
                $"{OfficialTradeConstants.OfficialTradeApiUrl}/whisper",
                Method.POST);

            request.AddJsonBody(new { token, values = new[] { mySaleAmount } });

            var response = GetRequest<WhisperResponse>(request);
            return response.IsSuccessful && response.Data.Success;
        }

        public SearchResponse SearchResultsByQuery(object query, string league)
        {
            var request = CreateRequest(
                $"{OfficialTradeConstants.OfficialTradeApiUrl}/search/{league}",
                Method.POST);

            request.AddJsonBody(query);

            var response = GetRequest<SearchResponse>(request);
            ThrowIfRequestFailed(response);

            return response.Data;
        }

        SearchResponse SearchResults(string league, ref string queryId, bool exchange)
        {
            var section = exchange
                ? "exchange"
                : "search";

            var request = CreateRequest(
                $"{OfficialTradeConstants.OfficialTradeUrl}/{section}/{league}/{queryId}",
                Method.GET);

            var response = _restClient.Execute(request);
            ThrowIfRequestFailed(response);

            var match = Regex.Match(response.Content, @"""state"":(.+)\n}", RegexOptions.Singleline);
            if (!match.Success)
            {
                throw new InvalidOperationException("Html content parsing error.");
            }

            var htmlQuery = match.Groups[1].Value;

            request = CreateRequest(
                $"{OfficialTradeConstants.OfficialTradeApiUrl}/{section}/{league}",
                Method.POST);

            if (exchange)
            {
                var query = JsonConvert.DeserializeObject<ExchangeSectionFilterHtml>(htmlQuery);
                var queryExchange = query.Exchange;

                var have = queryExchange.Have.Keys.FirstOrDefault();
                var want = queryExchange.Want.Keys.FirstOrDefault();
                var min = queryExchange.Minimum;

                return SearchExchange(league, have, want, min);
            }
            else
            {
                htmlQuery = Regex.Replace(htmlQuery, "\"disc\":\".+?\",", "");

                dynamic query = JsonConvert.DeserializeObject<ExpandoObject>(htmlQuery);
                query.status = new { option = "online" };
                var sort = new { price = "asc" };

                request.AddJsonBody(new { query, sort });
            }

            var result = GetRequest<SearchResponse>(request);
            ThrowIfRequestFailed(result);

            return result.Data;
        }

        public SearchResponseExchange SearchExchangeByQueryId(string league, ref string queryId)
        {
            return (SearchResponseExchange)SearchResults(league, ref queryId, true);
        }

        public SearchResponse SearchByQueryId(string league, ref string queryId)
        {
            return SearchResults(league, ref queryId, false);
        }

        public SearchResponseExchange SearchExchange(string league,
                                                     string have,
                                                     string want,
                                                     int min = 1,
                                                     int delay = -1)
        {
            var request = CreateRequest(
                $"{OfficialTradeConstants.OfficialTradeApiUrl}/exchange/{league}",
                Method.POST);

            var exchangeRequest = new ExchangeRequest
            {
                Engine = "new",
                Query = new Exchange
                {
                    Have = have != null
                        ? new[] { have }
                        : Array.Empty<string>(),
                    Want = want != null
                        ? new[] { want }
                        : Array.Empty<string>(),
                    Minimum = min,
                    Status = new Status { Option = "online" }
                },
                Sort = new ExchangeSort { Have = "asc" }
            };
            
            request.AddJsonBody(exchangeRequest);

            var response = GetRequest<SearchResponseExchange>(request, delay);
            ThrowIfRequestFailed(response);

            return response.Data;
        }

        /// <inheritdoc cref="Fetch(string[],string)"/>
        public FetchResponse Fetch(IEnumerable<string> csvHashes,
                                   string queryId,
                                   int limit,
                                   bool exchange = false)
        {
            limit = Math.Min(
                exchange
                    ? MaxExchangePerFetch
                    : MaxItemsPerFetch,
                limit);

            return Fetch(csvHashes.Take(limit).ToArray(), queryId, exchange);
        }

        /// <summary>
        /// Fetch the items data from csvHashes.
        /// </summary>
        /// <param name="csvHashes">CSV Hashes (from websocket or search response).</param>
        /// <param name="queryId">Search hash (the hash in the search url after the league name).</param>
        /// <param name="exchange">Is exchange section or not.</param>
        public FetchResponse Fetch(string[] csvHashes, string queryId, bool exchange = false)
        {
            var request = CreateRequest("/fetch/" + string.Join(",", csvHashes), Method.GET);
            request.AddQueryParameter("query", queryId);

            if (exchange)
            {
                request.AddQueryParameter("exchange", "1");
            }

            var response = GetRequest<FetchResponse>(request);
            ThrowIfRequestFailed(response);

            return response.Data;
        }

        IRestResponse<T> GetRequest<T>(IRestRequest request, int overridenDelay = -1) where T : class
        {
            RateLimit?.Wait(overridenDelay);
            
            var response = _restClient.Execute<T>(request);

            AdjustRateLimit(response);
            AdjustUserAgent(response);
            
            return response;
        }

        void AdjustRateLimit(IRestResponse response)
        {
            var ipLimit = response.Headers.FirstOrDefault(x => x.Name == "X-Rate-Limit-Ip");
            var accountLimit =
                response.Headers.FirstOrDefault(x => x.Name == "X-Rate-Limit-Account");

            if (ipLimit != null || accountLimit != null)
            {
                RateLimit.ChangeInterval(accountLimit?.Value as string, ipLimit?.Value as string);
            }
        }

        void AdjustUserAgent(IRestResponse response)
        {
            if (response.StatusCode == HttpStatusCode.OK && _userAgent == null)
            {
                _userAgent = _restClient.UserAgent;
            }
        }

        static void ThrowIfRequestFailed(IRestResponse response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var reason = "Request failed. StatusCode: " + response.StatusCode;

                if (!string.IsNullOrEmpty(response.Content) &&
                    response.Content.Contains(@"""error"""))
                {
                    reason += "\n" + response.Content;
                }

                throw new HttpRequestException(reason);
            }
        }

        OfficialTradeConfiguration _configuration;
        readonly RestClient _restClient;
    }
}