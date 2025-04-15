namespace GOCAP.Repository.AutoMapper;

public class UserEntityMapperProfile : EntityMapperProfileBase
{
    public UserEntityMapperProfile()
    {
        CreateMap<User, UserEntity>()
             .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => JsonHelper.Serialize(src.Picture)));

        CreateMap<UserEntity, User>()
            .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => JsonHelper.Deserialize<Media?>(src.Picture)));
        CreateMap<QueryResult<User>, QueryResult<UserEntity>>().ReverseMap();
        CreateMap<Notification, NotificationEntity>().ReverseMap();
		CreateMap<UserBlockEntity, UserBlock>().ReverseMap();
    }
}
