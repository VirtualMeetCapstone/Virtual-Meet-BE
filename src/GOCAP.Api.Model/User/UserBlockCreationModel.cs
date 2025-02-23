namespace GOCAP.Api.Model;

public class UserBlockCreationModel
{
	public Guid BlockedUserId { get; set; }
	public Guid BlockedByUserId { get; set; }
}
