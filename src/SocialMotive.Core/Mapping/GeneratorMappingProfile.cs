using AutoMapper;
using SocialMotive.Core.Data.Generator;
using SocialMotive.Core.Model.Generator;

namespace SocialMotive.Core.Mapping
{
    /// <summary>
    /// AutoMapper profile for Generator area entity ↔ DTO mappings
    /// </summary>
    public class GeneratorMappingProfile : Profile
    {
        public GeneratorMappingProfile()
        {
            #region Template

            CreateMap<DbTemplate, TemplateSummary>();

            CreateMap<DbTemplate, TemplateDetail>()
                .ForMember(d => d.Layers, opt => opt.MapFrom(s => s.Layers));

            CreateMap<CreateTemplateRequest, DbTemplate>()
                .ForMember(d => d.TemplateId, opt => opt.Ignore())
                .ForMember(d => d.UserId, opt => opt.Ignore())
                .ForMember(d => d.IsPublished, opt => opt.Ignore())
                .ForMember(d => d.IsTemplate, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
                .ForMember(d => d.DeletedAt, opt => opt.Ignore())
                .ForMember(d => d.User, opt => opt.Ignore())
                .ForMember(d => d.Layers, opt => opt.Ignore())
                .ForMember(d => d.RenderJobs, opt => opt.Ignore());

            #endregion

            #region Layer

            CreateMap<DbLayer, Layer>();

            CreateMap<Layer, DbLayer>()
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
                .ForMember(d => d.Template, opt => opt.Ignore())
                .ForMember(d => d.Asset, opt => opt.Ignore());

            #endregion

            #region Asset

            CreateMap<DbAsset, Asset>();

            CreateMap<UploadAssetRequest, DbAsset>()
                .ForMember(d => d.AssetId, opt => opt.Ignore())
                .ForMember(d => d.UserId, opt => opt.Ignore())
                .ForMember(d => d.ImagePng, opt => opt.MapFrom(s => s.FileData))
                .ForMember(d => d.ImageMetaDataJson, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
                .ForMember(d => d.DeletedAt, opt => opt.Ignore())
                .ForMember(d => d.User, opt => opt.Ignore());

            #endregion

            #region RenderJob

            CreateMap<DbRenderJob, RenderJobStatus>();

            #endregion
        }
    }
}
