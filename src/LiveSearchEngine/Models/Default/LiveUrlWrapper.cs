﻿using LiveSearchEngine.Interfaces;

namespace LiveSearchEngine.Models.Default
{
    public class LiveUrlWrapper : ISearchUrlWrapper
    {
        public LiveUrlWrapper(SniperItem sniperItem)
        {
            _sniperItem = sniperItem;
        }

        public string Hash => _sniperItem.SearchHash;
        public string League => _sniperItem.League;

        public string SearchUrl => $"{GlobalConstants.OfficialTradeUrl}/search/{League}/{Hash}";
        public string WebSocketUrl => $"{GlobalConstants.OfficialTradeApiUrl.Replace("https", "wss")}/live/{League}/{Hash}";
        
        readonly SniperItem _sniperItem;
    }
}