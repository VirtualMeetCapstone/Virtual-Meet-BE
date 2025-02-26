namespace GOCAP.Api.Model;

public class RoomHashTagModel : EntityDateTrackingBase<Guid>
{
	public override Guid Id { get; set; }
	public RoomModel? Room { get; set; }
	public Guid HashTagId { get; set; }
	public HashTag? HashTag { get; set; }
}
