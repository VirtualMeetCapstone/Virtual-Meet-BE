namespace GOCAP.Repository;

public class UserEntityMapperProfile : EntityMapperProfileBase
{
    public UserEntityMapperProfile()
    {
        CreateMap<User, UserEntity>();
        CreateMap<UserEntity, User>();
    }
}
