namespace SocialMotive.Core.Model.Generator
{
    public class TemplateListResponse
    {
        public List<TemplateSummary> Templates { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
