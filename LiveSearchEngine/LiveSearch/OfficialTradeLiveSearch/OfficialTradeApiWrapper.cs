using System;
using System.Collections.Generic;
using System.Linq;
using LiveSearchEngine.Models.Poe.Fetch;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace LiveSearchEngine.LiveSearch.OfficialTradeLiveSearch
{
    /// <summary>
    /// Official trade site API wrapper.
    /// </summary>
    public class OfficialTradeApiWrapper
    {
        public OfficialTradeApiWrapper(OfficialTradeConfiguration configuration)
        {
            _configuration = configuration;

            _restClient = new RestClient(GlobalConstants.OfficialTradeApiUrl);
            _restClient.UseNewtonsoftJson();

            _restClient.AddDefaultHeader("User-Agent", "x");
        }

        /// <summary>
        /// Ip rate limit data.
        /// </summary>
        public RateLimitWrapper RateLimitWrapper { get; private set; }

        /// <inheritdoc cref="Fetch(string[],string)"/>
        /// <param name="limit">Limit of fetched hashes.</param>
        public FetchResponse Fetch(IEnumerable<string> csvHashes, string queryHash, int limit)
        {
            limit = Math.Min(20, limit);
            return Fetch(csvHashes.Take(limit).ToArray(), queryHash);
        }

        /// <summary>
        /// Fetch the items data from csvHashes.
        /// </summary>
        /// <param name="csvHashes">CSV Hashes (from websocket or search response).</param>
        /// <param name="queryHash">Search hash (the hash in the search url after the league name).</param>
        public FetchResponse Fetch(string[] csvHashes, string queryHash)
        {
            var request = new RestRequest("/fetch/" + string.Join(",", csvHashes), Method.GET);
            request.AddQueryParameter("query", queryHash);

            if (_configuration.PoeSessionId != null)
            {
                request.AddCookie(GlobalConstants.PoeSessionIdCookieName, _configuration.PoeSessionId);
            }

            var response = GetRequest<FetchResponse>(request);
            if (!response.IsSuccessful)
                return null;

            return response.Data;
        }

        IRestResponse<T> GetRequest<T>(IRestRequest request) where T : class
        {
            RateLimitWrapper?.Wait();
            return ConfigureRateLimitAndRequest<T>(request);
        }

        IRestResponse<T> ConfigureRateLimitAndRequest<T>(IRestRequest request) where T : class
        {
            var response = _restClient.Execute<T>(request);
            var header = response.Headers.FirstOrDefault(x => x.Name == "X-Rate-Limit-Ip");
            if (header != null && header.Value is string rateLimit)
            {
                if (RateLimitWrapper == null)
                    RateLimitWrapper = new RateLimitWrapper();

                RateLimitWrapper.ChangeInterval(rateLimit, _configuration.DelayFactor);
            }

            return response;
        }

        readonly OfficialTradeConfiguration _configuration;
        readonly RestClient _restClient;
    }
}