namespace GOCAP.Repository.AutoMapper;

public class PostEntityMapperProfile : EntityMapperProfileBase
{
    public PostEntityMapperProfile()
    {
        CreateMap<PostReaction, PostReactionEntity>().ReverseMap();
    }
}