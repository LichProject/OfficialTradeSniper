namespace LiveSearchEngine.Interfaces
{
    public interface ISniperItem
    {
        string Description { get; }
        ISearchUrlWrapper SearchUrlWrapper { get; }
    }
}