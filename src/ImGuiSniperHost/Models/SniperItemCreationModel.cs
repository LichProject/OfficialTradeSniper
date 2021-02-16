using LiveSearchEngine.Models;

namespace ImGuiSniperHost.Models
{
    public class SniperItemCreationModel
    {
        public SniperItemCreationModel()
        {
            Reset();
        }

        public SniperItemCreationModel(SniperItem sniperItem)
        {
            Description = sniperItem.Description ?? string.Empty;
            Hash = sniperItem.SearchHash ?? string.Empty;
            League = sniperItem.League ?? string.Empty;
        }

        public string Description;
        public string Hash;
        public string League;

        public void Reset()
        {
            Description = string.Empty;
            Hash = string.Empty;
            League = string.Empty;
        }

        public SniperItem ToSniperItemObject()
        {
            return new SniperItem(Description, Hash, League);
        }
    }
}