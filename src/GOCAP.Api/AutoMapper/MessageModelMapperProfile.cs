namespace GOCAP.Api.AutoMapper;

public class MessageModelMapperProfile : ModelMapperProfileBase
{
    public MessageModelMapperProfile()
    {
        CreateMap<Message, MessageCreationModel>().ReverseMap();
    }
}
