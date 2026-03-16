namespace SocialMotive.Core.Model.Admin
{
    public class ImportResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<ImportResult> Results { get; set; } = new();
    }
}
