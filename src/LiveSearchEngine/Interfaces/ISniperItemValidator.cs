namespace LiveSearchEngine.Interfaces
{
    public interface ISniperItemValidator
    {
        bool Validate(ISniperItem sniperItem, Item item, Listing listing);
    }
}