namespace GOCAP.Database;

[Table("Rooms")]
public class RoomEntity : EntitySqlBase
{
    [MaxLength(AppConstants.MaxLengthTopic)]
    public required string Topic { get; set; }
    [MaxLength(AppConstants.MaxLengthDescription)]
    public string? Description { get; set; }
    public int MaximumMembers { get; set; }
    public string? Medias { get; set; } 
    public RoomStatusType? Status { get; set; }
    public Guid OwnerId { get; set; }
    public UserEntity? Owner { get; set; }
    public ICollection<RoomEventEntity> Events { get; set; } = [];
    public ICollection<RoomTagEntity> Tags { get; set; } = [];
    public ICollection<RoomSettingEntity> Settings { get; set; } = [];
    public ICollection<RoomMemberEntity> Members { get; set; } = [];
}
