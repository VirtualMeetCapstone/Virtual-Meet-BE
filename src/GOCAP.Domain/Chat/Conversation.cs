namespace GOCAP.Domain;

public class Conversation
{
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public Media? UserPicture { get; set; }
    public Message? LastMessage { get; set; }
}
