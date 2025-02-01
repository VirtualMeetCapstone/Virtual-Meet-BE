namespace GOCAP.Api.AutoMapper;

public class RoomModelMapperProfile : ModelMapperProfileBase
{
    public RoomModelMapperProfile()
    {
        CreateMap<User, RoomMemberModel>().ReverseMap();
        CreateMap<Room, RoomModel>().ReverseMap();
        CreateMap<Room, RoomCreationModel>().ReverseMap();
        CreateMap<QueryResult<Room>, QueryResult<RoomModel>>().ReverseMap();

        // Mapper for room favourite
        CreateMap<RoomFavourite, RoomFavouriteModel>().ReverseMap();
        CreateMap<RoomFavourite, RoomFavouriteCreationModel>().ReverseMap();
        CreateMap<QueryResult<RoomFavourite>, QueryResult<RoomFavouriteModel>>().ReverseMap();
    }
}