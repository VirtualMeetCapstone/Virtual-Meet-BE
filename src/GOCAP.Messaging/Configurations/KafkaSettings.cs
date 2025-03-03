namespace GOCAP.Messaging;

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = string.Empty;
    public Dictionary<string, string> Topics { get; set; } = new()
    {
        { nameof(KafkaConstants.Topics.UserLogin), KafkaConstants.Topics.UserLogin },
    };
    public string GroupId { get; set; } = KafkaConstants.GroupId;
    public int ReplicationFactor { get; set; } = 1;
    public int Partitions { get; set; } = 3;
}
