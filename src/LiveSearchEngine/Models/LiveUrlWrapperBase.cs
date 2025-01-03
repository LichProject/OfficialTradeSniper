using LiveSearchEngine.LiveSearch.OfficialTradeLiveSearch;

namespace LiveSearchEngine.Models
{
    public class LiveUrlWrapperBase : ISearchUrlWrapper
    {
        private const string Pattern = "pathofexile\\.com\\/trade\\/(.+?)\\/(.+?)\\/(\\w+)";

        private string league;
        private string searchUrl;

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
            get => searchUrl;
            set
            {
                Match match = Regex.Match(value, Pattern);
                if (match.Success)
                {
                    searchUrl = value;
                    league = match.Groups[2].Value;
                    Hash = match.Groups[3].Value;
                    WebSocketRelative = $"live/{league}/{Hash}";
                }
                else
                {
                    searchUrl = string.Empty;
                }
            }
        }

        [JsonIgnore]
        public string WebSocketRelative { get; private set; }

        #endregion
    }
}