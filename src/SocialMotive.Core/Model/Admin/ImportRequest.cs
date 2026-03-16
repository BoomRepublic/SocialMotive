namespace SocialMotive.Core.Model.Admin
{
    public class ImportRequest
    {
        public string TableName { get; set; } = string.Empty;
        public string JsonData { get; set; } = string.Empty;
    }
}
