namespace GOCAP.Database;

[Table("HashTags")]
public class HashTagEntity : EntitySqlBase
{
    [MaxLength(AppConstants.MaxLengthName)]
    public string? Name { get; set; }
    public ICollection<RoomHashTagEntity> Rooms { get; set; } = [];
}
