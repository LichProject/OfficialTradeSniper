<!-- ABOUT THE PROJECT -->
## About The Project

Live searching library for https://www.pathofexile.com/trade/search or any other site you want, you can make your own live-search engine easily.

## Getting Started

### Installation

[NuGet Package](https://www.nuget.org/packages/LiveSearchEngine)

<!-- EXAMPLE -->
## Usage

1) Add it to your project dependencies.
2) Configure it:
````C#
var sessid = "123456acvbnmqwertkr5oyu854hnnynfsada";
var cfg = new OfficialTradeConfiguration(sessid);
var liveSearch = new LiveSearchWrapper<OfficialTradeLiveSearch, OfficialTradeConfiguration>(cfg);
liveSearch.ItemFound += OnItemFound;
liveSearch.AddSniperItems(...);
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
