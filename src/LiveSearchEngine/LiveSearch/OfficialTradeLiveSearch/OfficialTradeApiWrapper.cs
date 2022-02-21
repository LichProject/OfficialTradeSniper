using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using LiveSearchEngine.Interfaces;
using LiveSearchEngine.Models.Poe.Fetch;
using LiveSearchEngine.Models.Poe.Search;
using Newtonsoft.Json;
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
        static readonly string UserAgent = RandomUa.RandomUserAgent;

        public const int MaxItemsPerFetch = 10;
        public const int MaxExchangePerFetch = 20;

        public OfficialTradeApiWrapper(OfficialTradeConfiguration configuration, IRateLimit rateLimit)
        {
            UseNewConfiguration(configuration);

            RateLimit = rateLimit;

            _restClient = new RestClient(OfficialTradeConstants.OfficialTradeApiUrl);
            _restClient.UseNewtonsoftJson();
            _restClient.AddDefaultHeader("User-Agent", UserAgent);
        }

        public IRateLimit RateLimit { get; }

        public void UseNewConfiguration(OfficialTradeConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SearchResponse SearchResults(string league, ref string queryId, bool exchange)
        {
            var section = exchange
                ? "exchange"
                : "search";

            var html = new RestClient().Execute(
                new RestRequest($"{OfficialTradeConstants.OfficialTradeUrl}/{section}/{league}/{queryId}", Method.GET));

            if (!html.IsSuccessful)
            {
                throw new HttpRequestException();
            }

            var match = Regex.Match(html.Content, "\\\"state\":(.+),\"loggedIn\"");
            if (!match.Success)
            {
                throw new InvalidOperationException("Something wrong here");
            }

            var htmlQuery = match.Groups[1].Value;
            var request = new RestRequest($"{OfficialTradeConstants.OfficialTradeApiUrl}/{section}/{league}", Method.POST);

            if (exchange)
            {
                var query = JsonConvert.DeserializeObject<ExchangeSectionFilterHtml>(htmlQuery);
                var queryExchange = query.Exchange;

                var exchangeRequest = new ExchangeRequest
                {
                    Exchange = new Exchange
                    {
                        Have = queryExchange.Have.Select(x => x.Key).ToArray(),
                        Want = queryExchange.Want.Select(x => x.Key).ToArray(),
                        Minimum = queryExchange.Minimum,
                        Status = new Status { Option = query.Status }
                    }
                };

                request.AddJsonBody(exchangeRequest);
            }
            else
            {
                htmlQuery = Regex.Replace(htmlQuery, "\"disc\":\".+?\",", "");

                var query = JsonConvert.DeserializeObject(htmlQuery);
                var sort = new { price = "asc" };

                request.AddJsonBody(new { query, sort });
            }

            var response = GetRequest<SearchResponse>(request);
            if (!response.IsSuccessful)
            {
                throw new HttpRequestException();
            }

            return response.Data;
        }

        public SearchResponse SearchExchange(string league, string have, string want, int min = 1, int delay = -1)
        {
            var request = new RestRequest($"{OfficialTradeConstants.OfficialTradeApiUrl}/exchange/{league}", Method.POST);

            var exchangeRequest = new ExchangeRequest
            {
                Exchange = new Exchange
                {
                    Have = new[] { have }, Want = new[] { want }, Minimum = min, Status = new Status { Option = "online" }
                }
            };

            request.AddJsonBody(exchangeRequest);

            var response = GetRequest<SearchResponse>(request, delay);
            if (!response.IsSuccessful)
            {
                throw new HttpRequestException();
            }

            return response.Data;
        }

        /// <inheritdoc cref="Fetch(string[],string)"/>
        public FetchResponse Fetch(IEnumerable<string> csvHashes, string queryId, int limit, bool exchange = false)
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
            var request = new RestRequest("/fetch/" + string.Join(",", csvHashes), Method.GET);
            request.AddQueryParameter("query", queryId);

            if (exchange)
            {
                request.AddQueryParameter("exchange", "1");
            }

            if (_configuration.PoeSessionId != null)
            {
                request.AddCookie(OfficialTradeConstants.PoeSessionIdCookieName, _configuration.PoeSessionId);
            }

            var response = GetRequest<FetchResponse>(request);
            if (!response.IsSuccessful)
            {
                throw new HttpRequestException("Deserialization failed or request is denied.");
            }

            return response.Data;
        }

        IRestResponse<T> GetRequest<T>(IRestRequest request, int overridenDelay = -1) where T : class
        {
            RateLimit?.Wait(overridenDelay);
            return ConfigureRateLimitAndRequest<T>(request);
        }

        IRestResponse<T> ConfigureRateLimitAndRequest<T>(IRestRequest request) where T : class
        {
            var response = _restClient.Execute<T>(request);

            var ipLimit = response.Headers.FirstOrDefault(x => x.Name == "X-Rate-Limit-Ip");
            var accountLimit = response.Headers.FirstOrDefault(x => x.Name == "X-Rate-Limit-Account");

            if (ipLimit != null || accountLimit != null)
            {
                RateLimit.ChangeInterval(accountLimit?.Value as string, ipLimit?.Value as string);
            }

            return response;
        }

        OfficialTradeConfiguration _configuration;
        readonly RestClient _restClient;
    }
}