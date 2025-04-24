using MongoDB.Bson;

namespace GOCAP.Database;
[BsonCollection("RoomStatistics")]
public class RoomStatisticsEntity : EntityMongoBase
{
    public string RoomId { get; set; } 

    public string RoomTopic { get; set; }
    public string OwnerId { get; set; }
    public string OwnerName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int PeakUsers { get; set; }
    public DateTime? PeakTime { get; set; }

    public List<UserLogEntry> JoinLogs { get; set; } = new();
    public List<UserLogEntry> LeaveLogs { get; set; } = new();
}

public class UserLogEntry
{
    public UserInfo User { get; set; }
    public DateTime Time { get; set; }
}

    