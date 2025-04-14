namespace GOCAP.Api.AutoMapper
{
    public class AdminModelMapperProfile : ModelMapperProfileBase
    {
        public AdminModelMapperProfile()
        {
            CreateMap<LogoUpdateModel, LogoUpdate>()
            .ForMember(dest => dest.Media, opt => opt.MapFrom(src => ConvertMediaHelper.ConvertFormFileToMedia(src.Picture)));

            CreateMap<LogoUpdate, LogoUpdateModel>()
                .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => ConvertMediaHelper.ConvertMediaToFormFile(src.Media)));
        }
    }
}
