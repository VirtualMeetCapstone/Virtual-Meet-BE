namespace GOCAP.Api.AutoMapper;

public class PostMapperProfile : ModelMapperProfileBase
{
    public PostMapperProfile()
    {
        CreateMap<Media, MediaModel>();
        CreateMap<MediaModel, Media>();

        CreateMap<QueryResult<Post>, QueryResult<PostModel>>();
        CreateMap<QueryResult<PostModel>, QueryResult<Post>>();

        CreateMap<Post, PostModel>();
        CreateMap<PostModel, Post>();

        CreateMap<Post, PostCreationModel>();

        CreateMap<PostCreationModel, Post>()
            .ForMember(dest => dest.Medias, opt => opt.Ignore());
    }
}
