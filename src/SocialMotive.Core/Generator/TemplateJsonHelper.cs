using System.Text.Json;
using SocialMotive.Core.Model.Generator;

namespace SocialMotive.Core.Generator;

public static class TemplateJsonHelper
{
    private static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };

    public static TemplateData Deserialize(string? json)
        => string.IsNullOrEmpty(json)
            ? new TemplateData()
            : JsonSerializer.Deserialize<TemplateData>(json, Options) ?? new TemplateData();

    public static string Serialize(TemplateData data)
        => JsonSerializer.Serialize(data, Options);

    public static int NextLayerId(List<Layer> layers)
        => layers.Count > 0 ? layers.Max(l => l.LayerId) + 1 : 1;
}
