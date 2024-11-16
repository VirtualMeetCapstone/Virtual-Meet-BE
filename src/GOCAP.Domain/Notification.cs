namespace GOCAP.Domain;

public class Notification : DateObjectBase
{
    public string? Message { get; set; }

    public Guid UserId { get; set; }

    public User? User { get; set; }
}
