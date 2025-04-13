namespace GOCAP.Repository.AutoMapper;

public class AdminEntityMapperProfile : EntityMapperProfileBase
{
    public AdminEntityMapperProfile()
    {
        CreateMap<LogoUpdate, AdminLogoEntity>()
             .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => JsonHelper.Serialize(src.Media)));
        CreateMap<AdminLogoEntity, LogoUpdate>()
            .ForMember(dest => dest.Media, opt => opt.MapFrom(src => JsonHelper.Deserialize<Media>(src.Picture)));
    }
}
