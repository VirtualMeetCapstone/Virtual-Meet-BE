namespace GOCAP.Api.Model;

public class StoryViewModel
{
    public Guid Id { get; set; }
    public Guid StoryId { get; set; }
    public Guid ViewerId { get; set; }
    public long CreateTime { get; set; }
    public long LastModifyTime { get; set; }
}
