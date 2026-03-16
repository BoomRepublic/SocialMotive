namespace SocialMotive.Core.Model.Admin
{
    public class ImportResult
    {
        public int RowIndex { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
