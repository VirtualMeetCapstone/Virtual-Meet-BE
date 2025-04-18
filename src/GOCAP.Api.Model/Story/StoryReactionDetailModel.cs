namespace GOCAP.Api.Model;

public class StoryReactionDetailModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public UserStoryModel? User { get; set; }
    public long CreateTime { get; set; }
}

