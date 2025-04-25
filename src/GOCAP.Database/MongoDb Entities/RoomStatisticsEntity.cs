using MongoDB.Bson;
using MongoDB.Bson.Serialization.Options;

namespace GOCAP.Database;
[BsonCollection("RoomStatistics")]
public class RoomStatisticsEntity : EntityMongoBase
{
    public string RoomId { get; set; }

    public string RoomTopic { get; set; }
    public string OwnerId { get; set; }
    public string OwnerName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int PeakUsers { get; set; }
    public DateTime? PeakTime { get; set; }

    public List<UserLogEntry> JoinLogs { get; set; } = new();
    public List<UserLogEntry> LeaveLogs { get; set; } = new();

    public int TotalUniqueUsers { get; set; }
    public double UserParticipationRate { get; set; }
    public int TotalJoins { get; set; }
    public int TotalLeaves { get; set; }
    public int CurrentUsersCount { get; set; }

    public long? DurationFromFirstJoinTicks { get; set; }
    public long? LongestSessionTicks { get; set; }
    
    public long? AverageUserSessionTicks { get; set; }
    public long? DurationTicks { get; set; }
    [BsonDictionaryOptions(DictionaryRepresentation.Document)]
    public Dictionary<string, int> UserSessionCounts { get; set; }
    public DateTime? FirstJoinTime { get; set; }
    public string LastJoinUserId { get; set; }
    public string FirstJoinUserId { get; set; }
}

public class UserLogEntry
{
    public UserInfo User { get; set; }
    public DateTime? Time { get; set; }
}

