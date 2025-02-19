using GOCAP.Database.Common.Entities;

namespace GOCAP.Database;

[Table("RoomChannels")]
public class RoomChannelEntity : EntitySqlBase, IUserTracking
{
    [MaxLength(AppConstants.MaxLengthName)]
    public required string Name { get; set; }
    [MaxLength(AppConstants.MaxLengthDescription)]
    public string? Description { get; set; }
    public ChannelType? Type { get; set; }
    public ChannelStatusType? Status { get; set; }
    public string? Picture {  get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? ModifiedBy { get; set; }
    public UserEntity? CreateUser { get; set; }
    public UserEntity? ModifyUser { get; set; }
    public ICollection<RoomEntity> Rooms { get; set; } = [];
}