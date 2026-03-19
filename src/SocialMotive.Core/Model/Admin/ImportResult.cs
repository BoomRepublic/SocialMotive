using Swashbuckle.AspNetCore.Annotations;

namespace SocialMotive.Core.Model.Admin
{
    [SwaggerSchema("AdminImportResult")]
    public class ImportResult
    {
        public int RowIndex { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
