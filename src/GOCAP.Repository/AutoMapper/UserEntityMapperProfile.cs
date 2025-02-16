namespace GOCAP.Repository.AutoMapper;

public class UserEntityMapperProfile : EntityMapperProfileBase
{
    public UserEntityMapperProfile()
    {
        CreateMap<User, UserEntity>()
             .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => JsonHelper.Serialize(src.Picture)));

        CreateMap<UserEntity, User>()
            .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => JsonHelper.Deserialize<Media?>(src.Picture)));

        CreateMap<UserNotification, UserNotificationEntity>().ReverseMap();
    }
}
