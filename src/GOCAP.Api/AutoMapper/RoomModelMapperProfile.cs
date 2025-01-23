namespace GOCAP.Api.AutoMapper;

public class RoomModelMapperProfile : ModelMapperProfileBase
{
    public RoomModelMapperProfile()
    {
        CreateMap<User, MemberModel>().ReverseMap();
        CreateMap<Room, RoomModel>().ReverseMap();
        CreateMap<Room, RoomCreationModel>().ReverseMap();
        CreateMap<QueryResult<Room>, QueryResult<RoomModel>>().ReverseMap();
    }
}