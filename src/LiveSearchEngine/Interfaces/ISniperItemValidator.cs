using LiveSearchEngine.Models.Poe;
using LiveSearchEngine.Models.Poe.Fetch;

namespace LiveSearchEngine.Interfaces
{
    public interface ISniperItemValidator
    {
        bool Validate(ISniperItem sniperItem, Item item, Listing listing);
    }
}