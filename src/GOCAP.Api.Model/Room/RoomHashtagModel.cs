namespace GOCAP.Api.Model;

public class RoomHashtagModel : EntityDateTrackingBase<Guid>
{
	public override Guid Id { get; set; }
	public RoomModel? Room { get; set; }
	public Guid HashTagId { get; set; }
	public Hashtag? HashTag { get; set; }
}
