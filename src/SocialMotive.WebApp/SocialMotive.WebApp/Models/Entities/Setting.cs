namespace SocialMotive.WebApp.Models
{
    public class Setting
    {
        public int SettingId { get; set; }
        public string SettingKey { get; set; } = string.Empty;
        public string SettingValue { get; set; } = string.Empty;
        public string? Scope { get; set; }
    }
}
