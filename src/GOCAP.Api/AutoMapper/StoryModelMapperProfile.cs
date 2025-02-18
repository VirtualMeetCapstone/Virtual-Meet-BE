namespace GOCAP.Api.AutoMapper;

public class StoryModelMapperProfile : ModelMapperProfileBase
{
    public StoryModelMapperProfile()
    {
        // Story
        CreateMap<Story, StoryModel>().ReverseMap();
        CreateMap<Story, StoryCreationModel>().ReverseMap();

        CreateMap<Story, StoryCreationModel>()
            .ForMember(dest => dest.MediaUpload, opt => opt.MapFrom(src => ConvertMediaToFormFile(src.MediaUpload)));

        CreateMap<StoryCreationModel, Story>()
            .ForMember(dest => dest.MediaUpload, opt => opt.MapFrom(src => ConvertFormFileToMedia(src.MediaUpload)));

        CreateMap<User, StoryUserModel>().ReverseMap();
        CreateMap<QueryResult<Story>, QueryResult<StoryModel>>().ReverseMap();

        CreateMap<Story, StoryDetailModel>().ReverseMap();

        // Story reaction
        CreateMap<StoryReaction, StoryReactionModel>().ReverseMap();
        CreateMap<StoryReaction, StoryReactionCreationModel>().ReverseMap();
        CreateMap<StoryReaction, StoryReactionDetailModel>().ReverseMap();
        CreateMap<QueryResult<StoryReaction>, QueryResult<StoryReactionDetailModel>>().ReverseMap();

        // Story view
        CreateMap<StoryView, StoryViewModel>().ReverseMap();
        CreateMap<StoryView, StoryViewCreationModel>().ReverseMap();
        CreateMap<User, StoryViewerModel>().ReverseMap();
        CreateMap<StoryViewDetail, StoryViewDetailModel>().ReverseMap();
        CreateMap<QueryResult<StoryViewDetail>, QueryResult<StoryViewDetailModel>>().ReverseMap();
    }
}
