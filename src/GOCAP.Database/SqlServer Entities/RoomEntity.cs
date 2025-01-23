namespace GOCAP.Database;

[Table("Rooms")]
public class RoomEntity : EntitySqlBase
{
    public string Topic { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int MaximumMembers { get; set; }
    public string? Medias { get; set; } // Save in json format
    public RoomStatusType? Status { get; set; }
    public Guid OwnerId { get; set; }
    public UserEntity? Owner { get; set; }
    public long CreateTime { get; set; }
    public long LastModifyTime { get; set; }
    public ICollection<RoomEventEntity> Events { get; set; } = [];
    public ICollection<RoomTagEntity> Tags { get; set; } = [];
    public ICollection<RoomSettingEntity> Settings { get; set; } = [];
    public ICollection<RoomMemberEntity> Members { get; set; } = [];
}
