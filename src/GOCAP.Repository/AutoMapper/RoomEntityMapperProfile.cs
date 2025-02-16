namespace GOCAP.Repository.AutoMapper;

public class RoomEntityMapperProfile : EntityMapperProfileBase
{
    public RoomEntityMapperProfile()
    {
        CreateMap<Room, RoomEntity>()
             .ForMember(dest => dest.Medias, opt => opt.MapFrom(src => JsonHelper.Serialize(src.Medias)));
        CreateMap<RoomEntity, Room>()
            .ForMember(dest => dest.Medias, opt => opt.MapFrom(src => JsonHelper.Deserialize<List<Media>>(src.Medias)));

        CreateMap<RoomFavourite, RoomFavouriteEntity>().ReverseMap();
    }
}