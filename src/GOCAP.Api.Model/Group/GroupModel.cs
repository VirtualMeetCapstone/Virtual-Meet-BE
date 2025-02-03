namespace GOCAP.Api.Model;

public class GroupModel
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public required string Name { get; set; } 
    public string? Picture { get; set; }
    public long CreateTime { get; set; }
    public long LastModifyTime { get; set; }
}


