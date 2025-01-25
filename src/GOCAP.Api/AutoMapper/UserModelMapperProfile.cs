namespace GOCAP.Api.AutoMapper;

public class UserModelMapperProfile : ModelMapperProfileBase
{
    public UserModelMapperProfile()
    {
        CreateMap<User, UserModel>().ReverseMap();
        CreateMap<User, UserProfileModel>().ReverseMap();
        CreateMap<User, UserCreationModel>().ReverseMap();

        CreateMap<GoogleUser, User>();
        CreateMap<User, GoogleUser>();

        CreateMap<FacebookUser, User>();
        CreateMap<User, FacebookUser>();
    }
}
