using LiveSearchEngine.Models;
using LiveSearchEngine.Models.Poe;
using LiveSearchEngine.Models.Poe.Fetch;

namespace LiveSearchEngine.Interfaces
{
    public interface ISniperItemValidator
    {
        bool Validate(SniperItem sniperItem, Item item, Listing listing);
    }
}