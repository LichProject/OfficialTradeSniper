namespace LiveSearchEngine.Interfaces
{
    public interface ISearchUrlWrapper
    {
        string Hash { get; }
        string SearchUrl { get; }
        string WebSocketUrl { get; }
    }
}