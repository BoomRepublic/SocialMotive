namespace SocialMotive.WebApp.Components.Layout;

public static class AppThemes
{
    public const string DefaultTheme = LightThemeValue;

    public const string LightThemeValue = "light";
    public const string DarkThemeValue = "dark";

    public static IReadOnlyList<AppTheme> All { get; } = new List<AppTheme>
    {
        new("Light", LightThemeValue, "https://blazor.cdn.telerik.com/blazor/13.0.0/kendo-theme-material/swatches/material-main.css"),
        new("Dark", DarkThemeValue, "https://blazor.cdn.telerik.com/blazor/13.0.0/kendo-theme-material/swatches/material-main-dark.css")
    };

    public static AppTheme GetByValue(string? value)
    {
        return All.FirstOrDefault(theme => string.Equals(theme.Value, value, StringComparison.OrdinalIgnoreCase))
            ?? All.First(theme => theme.Value == DefaultTheme);
    }
}
