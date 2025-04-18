namespace GOCAP.Api.AutoMapper
{
    public class LogoModelMapperProfile : ModelMapperProfileBase
    {
        public LogoModelMapperProfile()
        {
            CreateMap<LogoModel, Logo>()
            .ForMember(dest => dest.MediaUpload, opt => opt.MapFrom(src => ConvertMediaHelper.ConvertFormFileToMedia(src.MediaUpload)));

            CreateMap<Logo, LogoModel>()
                .ForMember(dest => dest.MediaUpload, opt => opt.MapFrom(src => ConvertMediaHelper.ConvertMediaToFormFile(src.MediaUpload)));
        }
    }
}
