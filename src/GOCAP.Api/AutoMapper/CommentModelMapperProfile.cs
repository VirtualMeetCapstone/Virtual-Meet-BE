namespace GOCAP.Api.AutoMapper;

public class CommentModelMapperProfile : ModelMapperProfileBase
{
    public CommentModelMapperProfile()
    {
        CreateMap<Comment, CommentModel>().ReverseMap();
        CreateMap<Comment, CommentCreationModel>().ReverseMap();
        CreateMap<CommentReaction, CommentReactionModel>().ReverseMap();
        CreateMap<QueryResult<Comment>, QueryResult<CommentModel>>().ReverseMap();
    }
}
