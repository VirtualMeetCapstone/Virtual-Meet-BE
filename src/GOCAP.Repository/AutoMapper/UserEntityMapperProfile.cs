namespace GOCAP.Repository.AutoMapper;

public class UserEntityMapperProfile : EntityMapperProfileBase
{
    public UserEntityMapperProfile()
    {
        CreateMap<User, UserEntity>();
        CreateMap<UserEntity, User>();
    }
}
