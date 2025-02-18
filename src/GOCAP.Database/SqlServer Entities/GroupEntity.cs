namespace GOCAP.Database;

[Table("Groups")]
public class GroupEntity : EntitySqlBase
{
    public Guid OwnerId { get; set; }
    public UserEntity? Owner { get; set; }
    [MaxLength(AppConstants.MaxLengthName)]
    public required string Name { get; set; }
    [MaxLength(AppConstants.MaxLengthUrl)]
    public string? Picture { get; set; }
    public ICollection<GroupMemberEntity> Members { get; set; } = [];
}
