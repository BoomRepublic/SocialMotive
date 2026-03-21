namespace SocialMotive.Core.Model.Generator;

/// <summary>
/// Represents the design-time data stored in the TemplateJson column.
/// Contains layer definitions as a JSON blob.
/// </summary>
public class TemplateData
{
    public List<Layer> Layers { get; set; } = new();
}
