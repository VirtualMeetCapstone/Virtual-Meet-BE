namespace GOCAP.Repository;

public class MediaEntityMapperProfile : EntityMapperProfileBase
{
    public MediaEntityMapperProfile()
    {
        CreateMap<Media, MediaEntity>();
        CreateMap<MediaEntity, Media>();
    }
}
