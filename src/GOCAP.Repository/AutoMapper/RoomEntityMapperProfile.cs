namespace GOCAP.Repository.AutoMapper;

public class RoomEntityMapperProfile : EntityMapperProfileBase
{
    public RoomEntityMapperProfile()
    {
        CreateMap<Room, RoomEntity>().ReverseMap();
        CreateMap<RoomFavourite, RoomFavouriteEntity>().ReverseMap();
    }
}