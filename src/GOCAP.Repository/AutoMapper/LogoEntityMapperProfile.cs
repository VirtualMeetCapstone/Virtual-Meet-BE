namespace GOCAP.Repository.AutoMapper;

public class LogoEntityMapperProfile : EntityMapperProfileBase
{
    public LogoEntityMapperProfile()
    {
        CreateMap<Logo, LogoEntity>()
            .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => JsonHelper.Serialize(src.Media)));

        CreateMap<LogoEntity, Logo>()
            .ForMember(dest => dest.Media, opt => opt.MapFrom(src => JsonHelper.Deserialize<Media>(src.Picture)));
    }
}
