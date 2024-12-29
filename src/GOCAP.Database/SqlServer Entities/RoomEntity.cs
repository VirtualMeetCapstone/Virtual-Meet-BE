namespace GOCAP.Database;

[Table("Rooms")]
public class RoomEntity : EntitySqlBase
{
    public string Topic { get; set; } = string.Empty;
    public string? Discription { get; set; }
    public int MaximumMembers {  get; set; }
    public string? Medias { get; set; } // Save in json format
    public long CreateTime { get; set; }
    public long LastModifyTime { get; set; }
    public IEnumerable<RoomLikeEntity> RoomLikes { get; set; } = [];
    public IEnumerable<RoomEventEntity> RoomEvents { get; set; } = [];
    public IEnumerable<RoomInvitationEntity> RoomInvitations { get; set; } = [];
    public IEnumerable<RoomTagEntity> RoomTags { get; set; } = [];
    public IEnumerable<RoomSettingEntity> RoomSettings { get; set; } = [];
    public IEnumerable<UserEntity> Members { get; set; } = [];
}
