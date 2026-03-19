using Swashbuckle.AspNetCore.Annotations;

namespace SocialMotive.Core.Model.Admin
{
    [SwaggerSchema("AdminImportResponse")]
    public class ImportResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<ImportResult> Results { get; set; } = new();
    }
}
