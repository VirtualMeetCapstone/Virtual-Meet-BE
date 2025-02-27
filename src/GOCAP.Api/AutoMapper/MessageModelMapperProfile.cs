namespace GOCAP.Api.AutoMapper;

public class MessageModelMapperProfile : ModelMapperProfileBase
{
    public MessageModelMapperProfile()
    {
        // Room 
        CreateMap<RoomMessage, RoomMessageModel>().ReverseMap();
        CreateMap<RoomMessage, RoomMessageCreationModel>().ReverseMap();

        // User
        CreateMap<UserMessage, UserMessageModel>().ReverseMap();
        CreateMap<UserMessage, UserMessageCreationModel>().ReverseMap();

        //Group
        CreateMap<GroupMessage, GroupMessageModel>().ReverseMap();
        CreateMap<GroupMessage, GroupMessageCreationModel>().ReverseMap();
    }
}
