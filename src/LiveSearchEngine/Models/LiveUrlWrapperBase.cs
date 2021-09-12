using System.Text.RegularExpressions;
using LiveSearchEngine.Interfaces;
using LiveSearchEngine.LiveSearch.OfficialTradeLiveSearch;
using Newtonsoft.Json;

namespace LiveSearchEngine.Models
{
    public class LiveUrlWrapperBase : ISearchUrlWrapper
    {
        const string Pattern = "pathofexile\\.com\\/trade\\/(.+?)\\/(.+?)\\/(\\w+)";

        public LiveUrlWrapperBase(string url)
        {
            SearchUrl = url;
        }

        public LiveUrlWrapperBase()
        {
        }

        #region Implementation of ISearchUrlWrapper

        public string Hash { get; private set; }

        public string SearchUrl
        {
            get => _searchUrl;
            set
            {
                var match = Regex.Match(value, Pattern);
                if (match.Success)
                {
                    _searchUrl = value;
                    _league = match.Groups[2].Value;
                    Hash = match.Groups[3].Value;
                    WebSocketUrl = $"{_apiUrl}/live/{_league}/{Hash}";
                }
                else
                {
                    _searchUrl = string.Empty;
                }
            }
        }

        [JsonIgnore]
        public string WebSocketUrl { get; private set; }

        #endregion

        string _league;
        string _searchUrl;

        readonly string _apiUrl = OfficialTradeConstants.OfficialTradeApiUrl.Replace("https", "wss");
    }
}