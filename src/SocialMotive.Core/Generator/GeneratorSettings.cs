namespace SocialMotive.Core.Data.Generator
{
    public class GeneratorSettings
    {
        public int TemplateId { get; set; }
        public List<DbLayer> Layers { get; set; } = new List<DbLayer>();
    }
}
