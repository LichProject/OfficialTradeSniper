using System;
using LiveSearchEngine.Models;

namespace LiveSearchEngine.Delegates
{
    public delegate void WebSocketDisconnectedDelegate(SniperItem sniperItem, Exception exception);
}