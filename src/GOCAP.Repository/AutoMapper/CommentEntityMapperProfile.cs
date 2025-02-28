namespace GOCAP.Repository.AutoMapper;

public class CommentEntityMapperProfile : EntityMapperProfileBase
{
    public CommentEntityMapperProfile()
    {
        CreateMap<Comment, CommentEntity>().ReverseMap();
        CreateMap<CommentAuthor, CommentAuthorEntity>().ReverseMap();
        CreateMap<QueryResult<Comment>, QueryResult<CommentEntity>>().ReverseMap();
        CreateMap<CommentReaction, CommentReactionEntity>().ReverseMap();
    }
}
