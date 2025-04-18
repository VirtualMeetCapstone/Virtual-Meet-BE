namespace GOCAP.Api.AutoMapper;

public class NotificationModelMapperProfile : ModelMapperProfileBase
{
    public NotificationModelMapperProfile()
    {
        CreateMap<NotificationSource, NotificationSourceModel>().ReverseMap();
        CreateMap<NotificationActor, NotificationActorModel>().ReverseMap();
        CreateMap<Notification, NotificationModel>().ReverseMap();
        CreateMap<QueryResult<Notification>, QueryResult<NotificationModel>>().ReverseMap();
    }
}
