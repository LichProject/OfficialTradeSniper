namespace LiveSearchEngine.Models;

public class WebSocketConnection : IDisposable
{
    private readonly ClientWebSocket webSocket;
    private readonly Uri uri;
    private bool disposed;

    public WebSocketConnection(ISniperItem sniperItem, ClientWebSocket webSocket, Uri uri)
    {
        SniperItem = sniperItem;
        this.webSocket = webSocket;
        this.uri = uri;
    }

    public ISniperItem SniperItem { get; }

    public event MessageWsEvent OnMessage;
    public event ErrorWsEvent OnError;
    public event CloseWsEvent OnClose;

    public bool IsConnected => State == WebSocketState.Open;

    public WebSocketState State => webSocket.State;

    public async Task<bool> ConnectAsync()
    {
        try
        {
            if (webSocket.State != WebSocketState.Open)
            {
                await webSocket.ConnectAsync(uri, CancellationToken.None);
                _ = Task.Factory.StartNew(ListenAsync, TaskCreationOptions.LongRunning);
            }

            return webSocket.State == WebSocketState.Open;
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex);
            return false;
        }
    }

    public async Task CloseAsync()
    {
        if (disposed || webSocket.State != WebSocketState.Open)
        {
            return;
        }

        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
    }

    private async Task ListenAsync()
    {
        byte[] buffer = new byte[8192];
        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    OnClose?.Invoke(result.CloseStatusDescription);
                    break;
                }

                string message = Encoding.UTF8.GetString(buffer.Take(result.Count).ToArray());
                OnMessage?.Invoke(message);
            }
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex);
        }
    }

    public void Dispose()
    {
        webSocket?.Dispose();
        disposed = true;
    }
}
