using System.Text.Json;

namespace GOCAP.Repository.AutoMapper;

public abstract class EntityMapperProfileBase : Profile
{
    protected static string SerializeMediaList(List<Media>? medias)
    {
        return medias != null && medias.Count > 0
            ? JsonSerializer.Serialize(medias)
            : string.Empty;
    }

    protected static string SerializeMedia(Media? media)
    {
        return media != null ? JsonSerializer.Serialize(media) : string.Empty;
    }

    protected static List<Media> DeserializeMediaList(string? jsonMedias)
    {
        return !string.IsNullOrEmpty(jsonMedias)
            ? JsonSerializer.Deserialize<List<Media>>(jsonMedias) ?? []
            : [];
    }
}
