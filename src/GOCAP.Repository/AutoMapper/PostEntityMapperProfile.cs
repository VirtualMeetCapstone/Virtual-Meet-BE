namespace GOCAP.Repository.AutoMapper;

public class PostEntityMapperProfile : EntityMapperProfileBase
{
    public PostEntityMapperProfile()
    {
        CreateMap<PostLike, UserPostLikeEntity>().ReverseMap();
    }
}