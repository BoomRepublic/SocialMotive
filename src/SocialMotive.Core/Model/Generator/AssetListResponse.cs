namespace SocialMotive.Core.Model.Generator
{
    public class AssetListResponse
    {
        public List<Asset> Assets { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
