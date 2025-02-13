using System.Text.Json;

namespace GOCAP.Repository.AutoMapper;

public class RoomEntityMapperProfile : EntityMapperProfileBase
{
    public RoomEntityMapperProfile()
    {
        CreateMap<Room, RoomEntity>()
             .ForMember(dest => dest.Medias, opt => opt.MapFrom(src => SerializeMediaList(src.Medias)));
        CreateMap<RoomEntity, Room>()
            .ForMember(dest => dest.Medias, opt => opt.MapFrom(src => DeserializeMediaList(src.Medias)));

        CreateMap<RoomFavourite, RoomFavouriteEntity>().ReverseMap();
    }
    protected static string SerializeMediaList(List<Media>? medias)
    {
        return medias != null && medias.Count > 0
            ? JsonSerializer.Serialize(medias)
            : string.Empty;
    }

    protected static List<Media> DeserializeMediaList(string? jsonMedias)
    {
        return !string.IsNullOrEmpty(jsonMedias)
            ? JsonSerializer.Deserialize<List<Media>>(jsonMedias) ?? []
            : [];
    }

}