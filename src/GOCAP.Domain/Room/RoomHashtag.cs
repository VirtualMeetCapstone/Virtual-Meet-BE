namespace GOCAP.Domain;

public class RoomHashtag : DateTrackingBase
{
	public Guid RoomId { get; set; }
	public Room? Room { get; set; }
	public Guid HashTagId { get; set; }
	public Hashtag? HashTag { get; set; }
}
