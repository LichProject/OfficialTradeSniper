using LiveSearchEngine.Interfaces;
using LiveSearchEngine.Models;
using LiveSearchEngine.Models.Poe;
using LiveSearchEngine.Models.Poe.Fetch;

namespace LiveSearchEngine.LiveSearch.Validators
{
    public class StockSizeValidator : ISniperItemValidator
    {
        #region Implementation of ISniperItemComparable

        public bool Validate(SniperItem sniperItem, Item item, Listing listing)
        {
            return item.StackSize.HasValue && sniperItem.CompareStock(item.StackSize.Value);
        }

        #endregion
    }
}