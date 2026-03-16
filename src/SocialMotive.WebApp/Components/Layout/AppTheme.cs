namespace SocialMotive.WebApp.Components.Layout;

public class AppTheme
{
    public string Text { get; }
    public string Value { get; }
    public string StylesheetUrl { get; }

    public AppTheme(string text, string value, string stylesheetUrl)
    {
        Text = text;
        Value = value;
        StylesheetUrl = stylesheetUrl;
    }
}
