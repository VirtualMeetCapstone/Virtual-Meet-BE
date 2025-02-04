namespace GOCAP.Domain;

public class RoomFavourite : DateObjectBase
{
    public Guid RoomId { get; set; }
    public Guid UserId { get; set; }
}
