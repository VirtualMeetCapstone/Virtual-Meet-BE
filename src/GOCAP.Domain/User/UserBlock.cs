namespace GOCAP.Domain;

public class UserBlock : DateTrackingBase
{
	public Guid Id { get; set; }
	public Guid BlockedUserId { get; set; }
	public Guid BlockedByUserId { get; set; }
	public User? BlockedUser { get; set; }
	public User? BlockedByUser { get; set; }
}
