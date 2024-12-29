namespace GOCAP.Database;

[BsonCollection("RequestJoinRooms")]
public class RequestJoinRoomEntity : EntityMongoBase
{
    public Guid RoomId { get; set; }

    public Guid UserId { get; set; }

    public string? RequestMessage { get; set; }

    public DateTime RequestedAt { get; set; }

    public string? Status { get; set; }
}
