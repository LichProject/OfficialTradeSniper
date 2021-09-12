<!-- ABOUT THE PROJECT -->
## About The Project

Live searching library for https://www.pathofexile.com/trade/search or any other site you want, you can make your own live-search engine easily.

## Getting Started

### Installation

[NuGet Package](https://www.nuget.org/packages/LiveSearchEngine)

<!-- USAGE EXAMPLES -->
## Usage

1) Add it to your project dependencies.
2) Configure it:
````C#
var sniperItems = new List<ISniperItem>();
var lsConfiguration = new OfficialTradeConfiguration
{
    PoeSessionId = "My poe session id"
};

var liveSearch = new LiveSearchWrapper<OfficialTradeLiveSearch, OfficialTradeConfiguration>(cfg);
liveSearch.ItemFound += OnItemFound;
liveSearch.AddSniperItems(sniperItems);
````

3) Run it:
````C#
var result = liveSearch.ConnectAll();
````

---

### Events:
````C#
liveSearch.Start
liveSearch.Stop
liveSearch.Connected
liveSearch.Reconnected
liveSearch.Disconnected
liveSearch.Error
````
---
### Change reconnection attempts (0 = disable reconnect):
````C#
liveSearch.Engine.ReconnectAttempts = 10;
````
---
## Creating your own live-search engine:
````C#
public class PoeTradeSearchUrlWrapper : ISearchUrlWrapper
{
    public PoeTradeSearchUrlWrapper(string wsUrl)
    {
        WebSocketUrl = wsUrl;
    }

    public string Hash { get; }
    public string SearchUrl { get; }
    public string WebSocketUrl { get; set; }
}

public class PoeTradeSniperItem : ISniperItem
{
    public PoeTradeSniperItem(string description, string wsUrl)
    {
        Description = description;
        SearchUrlWrapper = new PoeTradeSearchUrlWrapper(wsUrl);
    }

    public string Description { get; set; }
    public ISearchUrlWrapper SearchUrlWrapper { get; }
}

public class PoeTradeConfiguration : ILiveSearchConfiguration
{
    // Your configuration properties.
}

public class PoeTradeLiveSearch : LiveSearchEngineBase<PoeTradeConfiguration>
{
    public PoeTradeLiveSearch(PoeTradeConfiguration configuration)
        // null or your own IRateLimit if you need rate limitting.
        : base(configuration, null)
    {
    }
    
    public override bool ValidateConfiguration()
    {
        // Validate configuration here.
        return true;
    }

    protected override WebSocket CreateWebSocket(ISniperItem sniperItem)
    {
        // Create and return websocket here.
        var ws = new WebSocket(sniperItem.SearchUrlWrapper.WebSocketUrl);

        ws.Log.Level = LogLevel.Debug;
        ws.Compression = CompressionMethod.Deflate;

        return ws;
    }

    public event MyDelegate MyItemFound;

    protected override void WsOnMessageReceived(ISniperItem sniperItem, MessageEventArgs e)
    {
        string webSocketContent = e.Data;

        // Handle websocket messages.

        // Call item found event here.
        // Create your own event and subscribe to through liveSearch.Engine.MyEvent

        // Or reuse base event (ItemFound) -> RaiseItemFound(sniperItem, item, listing);
        // you will have to use Models.Poe.Item and Models.Poe.Listing
        
        MyItemFound?.Invoke(sniperItem, ... item data ...);
    }
}

var cfg = new PoeTradeConfiguration();
var liveSearch = new LiveSearchWrapper<PoeTradeLiveSearch, PoeTradeConfiguration>(cfg);
liveSearch.Engine.MyItemFound += OnMyItemFound;

var mySniperItem = new PoeTradeSniperItem("My sniper item", "wss://live.poe.trade/helloworld");
liveSearch.AddSniperItem(mySniperItem);
````

<!-- CONTACT -->
## Contact
Discord - Lich#0491
