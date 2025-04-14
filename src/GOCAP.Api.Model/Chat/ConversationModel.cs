namespace GOCAP.Api.Model;

public class ConversationModel
{
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public Media? UserPicture { get; set; }
    public MessageModel? LastMessage { get; set; }
}
