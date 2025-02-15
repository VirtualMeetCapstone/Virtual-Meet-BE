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

        CreateMap<QueryResult<Story>, QueryResult<StoryModel>>().ReverseMap();

        // Story reaction
        CreateMap<StoryReaction, StoryReactionModel>().ReverseMap();

        // Story view
        CreateMap<StoryView, StoryViewModel>().ReverseMap();
    }
}
