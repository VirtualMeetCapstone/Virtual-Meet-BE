namespace GOCAP.Repository.AutoMapper;

public class MessageEntityMapperProfile : EntityMapperProfileBase
{
    public MessageEntityMapperProfile()
    {
        CreateMap<RoomMessage, RoomMessageEntity>().ReverseMap();
        CreateMap<UserMessage, UserMessageEntity>().ReverseMap();
        CreateMap<GroupMessage, GroupMessageEntity>().ReverseMap();
    }
}
