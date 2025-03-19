namespace GOCAP.Common;
public class KafkaSettings
{
    public string BootstrapServers { get; set; } = string.Empty;
    public string SecurityProtocol { get; set; } = string.Empty;
    public string SaslMechanism { get; set; } = string.Empty;
    public string SaslUsername { get; set; } = string.Empty;
    public string SaslPassword { get; set; } = string.Empty;
    public Dictionary<string, string> Topics { get; set; } = new()
    {
        { nameof(KafkaConstants.Topics.UserLogin), KafkaConstants.Topics.UserLogin },
        { nameof(KafkaConstants.Topics.Notification), KafkaConstants.Topics.Notification },
    };
    public string GroupId { get; set; } = KafkaConstants.GroupId;
    public int ReplicationFactor { get; set; } = 1;
    public int Partitions { get; set; } = 3;
}
