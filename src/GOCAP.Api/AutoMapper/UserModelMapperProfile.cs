namespace GOCAP.Api.AutoMapper;

public class UserModelMapperProfile : ModelMapperProfileBase
{
    public UserModelMapperProfile()
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
