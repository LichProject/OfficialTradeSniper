using LiveSearchEngine.Models;
using LiveSearchEngine.Models.Poe;
using LiveSearchEngine.Models.Poe.Fetch;

namespace LiveSearchEngine.Delegates
{
    public delegate void ItemFoundDelegate(SniperItem sniperItem, Item item, Listing listing);
}