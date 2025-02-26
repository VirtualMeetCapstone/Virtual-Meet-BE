namespace GOCAP.Domain;

public class RoomHashTag : DateTrackingBase
{
	public Guid RoomId { get; set; }
	public Room? Room { get; set; }
	public Guid HashTagId { get; set; }
	public Hashtag? HashTag { get; set; }
}
