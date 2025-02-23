namespace GOCAP.Api.Model;

public class UserCreationModel
{
	public Guid BlockedUserId { get; set; }
	public Guid BlockedByUserId { get; set; }
}
