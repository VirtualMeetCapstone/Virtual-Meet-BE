namespace GOCAP.Api.AutoMapper;

public class PostModelMapperProfile : ModelMapperProfileBase
{
    public PostModelMapperProfile()
    {
        CreateMap<QueryResult<Post>, QueryResult<PostModel>>().ReverseMap();
        CreateMap<Post, PostModel>().ReverseMap();

        CreateMap<Post, PostCreationModel>()
            .ForMember(dest => dest.MediaUploads, opt => opt
            .MapFrom(src => ConvertMediasToFormFiles(src.MediaUploads)));

        CreateMap<PostCreationModel, Post>()
            .ForMember(dest => dest.MediaUploads, opt => opt
            .MapFrom(src => ConvertFormFilesToMedias(src.MediaUploads)));


        CreateMap<PostReaction, PostReactionCreationModel>().ReverseMap();
        CreateMap<PostReaction, PostReactionModel>().ReverseMap();
        CreateMap<UserReactionPost, UserReactionPostModel>().ReverseMap();
        CreateMap<QueryResult<UserReactionPost>, QueryResult<UserReactionPostModel>>().ReverseMap();
    }
}
