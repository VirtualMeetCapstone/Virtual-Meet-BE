namespace GOCAP.Domain;

public class UserLoginEvent
{
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public long LoginTime { get; set; }
}
