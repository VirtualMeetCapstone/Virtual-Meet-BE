namespace GOCAP.Database;

[Table("RoomMemberRoles")]
public class RoomMemberRoleEntity : EntitySqlBase
{
    public Guid RoomMemberId { get; set; }
    [MaxLength(GOCAPConstants.MaxLengthName)]
    public string? RoleName { get; set; }
    public RoomMemberEntity? RoomMember { get; set; }
}
