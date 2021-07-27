using System;
using LiveSearchEngine.Interfaces;

namespace LiveSearchEngine.Delegates
{
    public delegate void WebSocketDisconnectedDelegate(ISniperItem sniperItem, Exception exception);
}