namespace GOCAP.Repository.AutoMapper;

public class StoryEntityMapperProfile : EntityMapperProfileBase
{
    public StoryEntityMapperProfile()
    {
        // Story
        CreateMap<Story, StoryEntity>()
             .ForMember(dest => dest.Media, opt => opt.MapFrom(src => JsonHelper.Serialize(src.Media)));
        CreateMap<StoryEntity, Story>()
            .ForMember(dest => dest.Media, opt => opt.MapFrom(src => JsonHelper.Deserialize<Media>(src.Media)));
        CreateMap<QueryResult<Story>, QueryResult<StoryEntity>>().ReverseMap();

        // Story reaction
        CreateMap<StoryReaction, StoryReactionEntity>().ReverseMap();
        CreateMap<QueryResult<StoryReaction>, QueryResult<StoryReactionEntity>>().ReverseMap();
        
        // Story view
        CreateMap<StoryView, StoryViewEntity>().ReverseMap();
    }
}