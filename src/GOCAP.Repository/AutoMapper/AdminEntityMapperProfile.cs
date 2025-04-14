namespace GOCAP.Repository.AutoMapper;

public class AdminEntityMapperProfile : EntityMapperProfileBase
{
    public AdminEntityMapperProfile()
    {
        CreateMap<LogoUpdate, AdminLogoEntity>()
            .ForMember(dest => dest.Picture, opt => opt.MapFrom(src =>
                src.Media != null ? JsonHelper.Serialize(src.Media) : string.Empty));

        CreateMap<AdminLogoEntity, LogoUpdate>()
            .ForMember(dest => dest.Media, opt => opt.MapFrom(src =>
                !string.IsNullOrEmpty(src.Picture) ? JsonHelper.Deserialize<MediaUpload>(src.Picture) : null!));
    }
}
