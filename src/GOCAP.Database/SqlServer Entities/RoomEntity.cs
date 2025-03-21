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
    public PrivacyType Privacy { get; set; } = PrivacyType.Public;
    public RoomStatusType? Status { get; set; }
    public Guid OwnerId { get; set; }
    public UserEntity? Owner { get; set; }
    public string? PasswordHash {  get; set; }
    public bool RequireApproval { get; set; }
    [MaxLength(AppConstants.MaxLengthDescription)] 
    public string? WelcomeMessage { get; set; }
    public Guid? ChannelId { get; set; }
    public RoomChannelEntity? Channel { get; set; }
    public ICollection<RoomEventEntity> Events { get; set; } = [];
    public ICollection<RoomHashTagEntity> HashTags { get; set; } = [];
    public ICollection<RoomSettingEntity> Settings { get; set; } = [];
    public ICollection<RoomMemberEntity> Members { get; set; } = [];
    public ICollection<RoomTagEntity> Tags { get; set; } = [];
}
