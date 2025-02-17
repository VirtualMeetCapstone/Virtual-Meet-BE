namespace GOCAP.Domain;

public class RoomFavourite : DateTrackingBase
{
    public Guid RoomId { get; set; }
    public Guid UserId { get; set; }
}
