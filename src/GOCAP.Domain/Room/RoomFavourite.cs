namespace GOCAP.Domain;

public class RoomFavourite : DomainBase
{
    public Guid RoomId { get; set; }
    public Guid UserId { get; set; }
    public long CreateTime { get; set; }
}
