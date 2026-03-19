using Swashbuckle.AspNetCore.Annotations;

namespace SocialMotive.Core.Model.Admin
{
    [SwaggerSchema("AdminImportRequest")]
    public class ImportRequest
    {
        public string TableName { get; set; } = string.Empty;
        public string JsonData { get; set; } = string.Empty;
    }
}
