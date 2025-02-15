namespace GOCAP.Repository.AutoMapper;

public class StoryEntityMapperProfile : EntityMapperProfileBase
{
    public StoryEntityMapperProfile()
    {
        CreateMap<Story, StoryEntity>()
             .ForMember(dest => dest.Media, opt => opt.MapFrom(src => SerializeMedia(src.Media)));
        CreateMap<StoryEntity, Story>()
            .ForMember(dest => dest.Media, opt => opt.MapFrom(src => DeserializeMediaList(src.Media)));

        CreateMap<StoryReaction, StoryReactionEntity>().ReverseMap();
        CreateMap<StoryView, StoryViewEntity>().ReverseMap();
    }
}