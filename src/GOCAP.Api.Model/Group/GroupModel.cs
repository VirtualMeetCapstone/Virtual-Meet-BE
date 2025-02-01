namespace GOCAP.Api.Model;

public class GroupModel
{
    public Guid Id { get; set; }
    public GroupOwnerModel? Owner { get; set; }
    public required string Name { get; set; } 
    public string? Picture { get; set; }
    public long CreateTime { get; set; }
    public long LastModifyTime { get; set; }
    public ICollection<GroupMember> Members { get; set; } = [];
}


