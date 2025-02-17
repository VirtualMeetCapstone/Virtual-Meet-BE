namespace GOCAP.Api.Model;

public class RoomFavouriteModel : EntityDateTrackingBase<Guid>
{
    public override Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public Guid UserId { get; set; }
}
