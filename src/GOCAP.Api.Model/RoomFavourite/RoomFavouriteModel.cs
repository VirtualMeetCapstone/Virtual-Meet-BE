namespace GOCAP.Api.Model;

public class RoomFavouriteModel
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public Guid UserId { get; set; }
    public long CreateTime { get; set; }
}
