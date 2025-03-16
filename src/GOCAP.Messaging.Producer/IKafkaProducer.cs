namespace GOCAP.Messaging.Producer;

public interface IKafkaProducer
{
    Task ProduceAsync<T>(string topic, T message, CancellationToken cancellationToken = default);
}
