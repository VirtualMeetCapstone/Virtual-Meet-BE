namespace GOCAP.Database;

[Table("Groups")]
public class GroupEntity : EntitySqlBase
{
    public Guid OwnerId { get; set; }
    public UserEntity? Owner { get; set; }
    [MaxLength(GOCAPConstants.MaxLengthName)]
    public string? GroupName { get; set; }
    public ICollection<GroupMemberEntity> Members { get; set; } = [];
}
