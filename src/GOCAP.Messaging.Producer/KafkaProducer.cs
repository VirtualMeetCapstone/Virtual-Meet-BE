using Confluent.Kafka;
using GOCAP.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GOCAP.Messaging.Producer;

public class KafkaProducer : IKafkaProducer, IAsyncDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;
    public KafkaProducer(IOptions<KafkaSettings> kafkaSettings, ILogger<KafkaProducer> logger)
    {
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = kafkaSettings.Value.BootstrapServers,
            SecurityProtocol = SecurityProtocol.SaslSsl, 
            SaslMechanism = SaslMechanism.ScramSha256,   
            SaslUsername = kafkaSettings.Value.SaslUsername, 
            SaslPassword = kafkaSettings.Value.SaslPassword, 
            Acks = Acks.All 
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task ProduceAsync<T>(string topic, T message, CancellationToken cancellationToken = default)
    {
        if (message == null)
        {
            _logger.LogWarning("Message is null, skipping...");
            return;
        }
        var jsonMessage = JsonHelper.Serialize(message);
        var kafkaMessage = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = jsonMessage ?? string.Empty,
        };
        var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage, cancellationToken);
        _logger.LogInformation("Message sent to {Topic} | Partition: {Partition} | Offset: {Offset} | Key: {Key}",
        topic, deliveryResult.Partition, deliveryResult.Offset, kafkaMessage.Key);
    }

    public ValueTask DisposeAsync()
    {
        _logger.LogInformation("Disposing Kafka producer...");
        _producer.Dispose();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
