namespace SocialMotive.Core.Data
{
    public class DbSetting
    {
        public int SettingId { get; set; }
        public string SettingKey { get; set; } = string.Empty;
        public string SettingValue { get; set; } = string.Empty;
        public string? Scope { get; set; }
    }
}
