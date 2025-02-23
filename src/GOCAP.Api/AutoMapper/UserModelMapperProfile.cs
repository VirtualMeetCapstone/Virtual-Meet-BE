namespace GOCAP.Api.AutoMapper;

public class UserModelMapperProfile : ModelMapperProfileBase
{
	public UserModelMapperProfile()
	{
		CreateMap<User, UserModel>().ReverseMap();
		CreateMap<User, UserProfileModel>().ReverseMap();

		CreateMap<User, UserUpdationModel>()
			.ForMember(dest => dest.PictureUpload, opt => opt.MapFrom(src => ConvertMediaToFormFile(src.PictureUpload)));

		CreateMap<UserUpdationModel, User>()
			.ForMember(dest => dest.PictureUpload, opt => opt.MapFrom(src => ConvertFormFileToMedia(src.PictureUpload)));

		CreateMap<QueryResult<User>, QueryResult<UserModel>>().ReverseMap();
		CreateMap<User, ReferenceNotificationModel>().ReverseMap();
		CreateMap<UserNotification, UserNotificationModel>().ReverseMap();

		CreateMap<GoogleUser, User>();
		CreateMap<User, GoogleUser>();

		CreateMap<FacebookUser, User>();
		CreateMap<User, FacebookUser>();
		CreateMap<UserBlockCreationModel, UserBlock>().ReverseMap();
		CreateMap<UserBlock, UserBlockModel>()
			.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.BlockedUser != null ? src.BlockedUser.Name : string.Empty))
			.ForMember(dest => dest.Picture, opt => opt.MapFrom(src => src.BlockedUser != null ? src.BlockedUser.Picture : null))
			.ReverseMap();
	}
}
