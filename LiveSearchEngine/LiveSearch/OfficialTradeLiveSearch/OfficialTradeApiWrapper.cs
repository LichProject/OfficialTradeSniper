using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        /// <inheritdoc cref="FetchAsync(string[],string)"/>
        /// <param name="limit">Limit of fetched hashes.</param>
        public async Task<FetchResponse> FetchAsync(IEnumerable<string> csvHashes, string queryHash, int limit)
        {
            limit = Math.Min(20, limit);
            return await FetchAsync(csvHashes.Take(limit).ToArray(), queryHash);
        }

        /// <summary>
        /// Fetch the items data from csvHashes.
        /// </summary>
        /// <param name="csvHashes">CSV Hashes (from websocket or search response).</param>
        /// <param name="queryHash">Search hash (the hash in the search url after the league name).</param>
        public async Task<FetchResponse> FetchAsync(string[] csvHashes, string queryHash)
        {
            var request = new RestRequest("/fetch/" + string.Join(",", csvHashes), Method.GET);
            request.AddQueryParameter("query", queryHash);

            if (_configuration.PoeSessionId != null)
            {
                request.AddCookie(GlobalConstants.PoeSessionIdCookieName, _configuration.PoeSessionId);
            }

            var response = await RequestAsync<FetchResponse>(request);
            if (!response.IsSuccessful)
                return null;

            return response.Data;
        }

        async Task<IRestResponse<T>> RequestAsync<T>(RestRequest request) where T : class
        {
            if (RateLimitWrapper != null)
            {
                await RateLimitWrapper.WaitAsync();
            }

            return await ConfigureRateLimitAndRequestAsync<T>(request);
        }

        async Task<IRestResponse<T>> ConfigureRateLimitAndRequestAsync<T>(RestRequest request) where T : class
        {
            var response = await _restClient.ExecuteAsync<T>(request);
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