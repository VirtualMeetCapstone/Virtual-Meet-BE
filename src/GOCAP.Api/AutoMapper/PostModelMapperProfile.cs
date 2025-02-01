namespace GOCAP.Api.AutoMapper;

public class PostModelMapperProfile : ModelMapperProfileBase
{
    public PostModelMapperProfile()
    {
        CreateMap<Media, MediaModel>();
        CreateMap<MediaModel, Media>();

        CreateMap<QueryResult<UserPost>, QueryResult<PostModel>>();
        CreateMap<QueryResult<PostModel>, QueryResult<UserPost>>();

        CreateMap<UserPost, PostModel>().ReverseMap();
        CreateMap<UserPost, PostCreationModel>();
        CreateMap<PostCreationModel, UserPost>()
            .ForMember(dest => dest.Medias, opt => opt.Ignore());

        CreateMap<PostLike, PostLikeCreationModel>().ReverseMap();
        CreateMap<PostLike, PostLikeModel>().ReverseMap();
    }
}
