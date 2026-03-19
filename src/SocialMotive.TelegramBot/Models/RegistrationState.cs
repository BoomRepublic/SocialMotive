namespace SocialMotive.TelegramBot.Models;

public enum RegistrationStep
{
    AwaitingFirstName,
    AwaitingLastName,
    AwaitingEmail,
    AwaitingMobilePhone,
    AwaitingCity,
    AwaitingCitySelection,
    AwaitingConfirmation
}

public class RegistrationState
{
    public long ChatId { get; set; }
    public RegistrationStep Step { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? MobilePhone { get; set; }
    public int? CityId { get; set; }
    public string? CityName { get; set; }
    public string? TelegramUsername { get; set; }
    public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;
    public List<(int CityId, string CityName)> CitySearchResults { get; set; } = new();
}
