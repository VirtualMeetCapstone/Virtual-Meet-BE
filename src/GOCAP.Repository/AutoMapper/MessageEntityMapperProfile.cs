namespace GOCAP.Repository.AutoMapper;

public class MessageEntityMapperProfile : EntityMapperProfileBase
{
    public MessageEntityMapperProfile()
    {
        CreateMap<Message, MessageEntity>().ReverseMap();
    }
}
