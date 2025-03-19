namespace GOCAP.Repository.AutoMapper;

public class NotificationEntityMapperProfile : EntityMapperProfileBase
{
    public NotificationEntityMapperProfile()
    {
        CreateMap<NotificationSource, NotificationSourceEntity>().ReverseMap();
        CreateMap<NotificationActor, NotificationActorEntity>().ReverseMap();
        CreateMap<Notification, NotificationEntity>().ReverseMap();
        CreateMap<QueryResult<Notification>, QueryResult<NotificationEntity>>().ReverseMap();
    }
}
