namespace GOCAP.Api.AutoMapper;

public class PostModelMapperProfile : ModelMapperProfileBase
{
    public PostModelMapperProfile()
    {
        CreateMap<QueryResult<Post>, QueryResult<PostModel>>();
        CreateMap<QueryResult<PostModel>, QueryResult<Post>>();

        CreateMap<Post, PostModel>().ReverseMap();
        CreateMap<Post, PostCreationModel>();
        CreateMap<PostCreationModel, Post>()
            .ForMember(dest => dest.Medias, opt => opt.Ignore());

        CreateMap<PostReaction, PostReactionCreationModel>().ReverseMap();
        CreateMap<PostReaction, PostReactionModel>().ReverseMap();
    }
}
