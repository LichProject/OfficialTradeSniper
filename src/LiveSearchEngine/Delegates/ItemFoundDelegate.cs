using LiveSearchEngine.Interfaces;
using LiveSearchEngine.Models.Poe;
using LiveSearchEngine.Models.Poe.Fetch;

namespace LiveSearchEngine.Delegates
{
    public delegate void ItemFoundDelegate(ISniperItem sniperItem, Item item, Listing listing);
}