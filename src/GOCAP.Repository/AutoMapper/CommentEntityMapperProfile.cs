namespace GOCAP.Repository.AutoMapper;

public class CommentEntityMapperProfile : EntityMapperProfileBase
{
    public CommentEntityMapperProfile()
    {
        CreateMap<Comment, CommentEntity>().ReverseMap();
        CreateMap<CommentAuthor, CommentAuthorEntity>().ReverseMap();
        CreateMap<CommentContent, CommentContentEntity>().ReverseMap();
        CreateMap<QueryResult<Comment>, QueryResult<CommentEntity>>().ReverseMap();
    }
}
