namespace GOCAP.Api.AutoMapper;

public class UserMapperProfile : MapperProfileBase
{
    public UserMapperProfile()
    {
        CreateMap<User, UserModel>();
        CreateMap<UserModel, User>();
        CreateMap<User, UserCreationModel>();
        CreateMap<UserCreationModel, User>();

        CreateMap<GoogleUser, User>();
        CreateMap<User, GoogleUser>();

        CreateMap<FacebookUser, User>();
        CreateMap<User, FacebookUser>();
    }
}
