namespace GOCAP.Api.AutoMapper;

public class MediaModelMapperProfile : ModelMapperProfileBase
{
    public MediaModelMapperProfile()
    {
        CreateMap<Media, MediaModel>();
        CreateMap<MediaModel, Media>();
        CreateMap<Media, MediaCreationModel>();
        CreateMap<MediaCreationModel, Media>();
        CreateMap<MediaCreationModel, MediaModel>();
        CreateMap<MediaModel, MediaCreationModel>();
    }
}
