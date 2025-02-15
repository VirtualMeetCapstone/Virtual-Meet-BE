namespace GOCAP.Api.Model;

public class StoryReactionModel
{
    public Guid Id { get; set; }
    public Guid StoryId { get; set; }
    public Guid UserId { get; set; }
    public long CreateTime { get; set; }
    public long LastModifyTime { get; set; }
}
