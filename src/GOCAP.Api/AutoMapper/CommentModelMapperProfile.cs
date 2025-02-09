namespace GOCAP.Api.AutoMapper;

public class CommentModelMapperProfile : ModelMapperProfileBase
{
    public CommentModelMapperProfile()
    {
        CreateMap<Comment, CommentModel>().ReverseMap();
        CreateMap<Comment, CommentCreationModel>().ReverseMap();
        CreateMap<CommentContent, CommentContentModel>().ReverseMap();
        CreateMap<QueryResult<Comment>, QueryResult<CommentModel>>().ReverseMap();
    }
}
