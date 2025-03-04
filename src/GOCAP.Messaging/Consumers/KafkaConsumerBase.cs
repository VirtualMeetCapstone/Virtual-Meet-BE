using Microsoft.Extensions.Hosting;

namespace GOCAP.Messaging;

public abstract class KafkaConsumerBase : BackgroundService, IAsyncDisposable
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<KafkaConsumerBase> _logger;
    private readonly string _topic;

    public KafkaConsumerBase(IOptions<KafkaSettings> kafkaSettings, ILogger<KafkaConsumerBase> logger, string topic)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = kafkaSettings.Value.BootstrapServers,
            GroupId = kafkaSettings.Value.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true,
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
        _logger = logger;
        _topic = topic;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_topic);
        _logger.LogInformation("Started consuming from topic: {Topic}", _topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);
                    if (consumeResult?.Message?.Value != null)
                    {
                        _logger.LogInformation("Received message {Value} at offset {Offset}",
                            consumeResult.Message.Value, consumeResult.Offset);

                        await ProcessMessageAsync(consumeResult.Message.Value);
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError("Kafka consume error: {Error}", ex.Error.Reason);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Consumer for {Topic} is stopping...", _topic);
        }
        finally
        {
            _consumer.Close();
            _logger.LogInformation("Consumer for {Topic} has stopped.", _topic);
        }
    }
    protected abstract Task ProcessMessageAsync(string message);

    public ValueTask DisposeAsync()
    {
        _logger.LogInformation("Disposing Kafka producer...");
        _consumer.Dispose();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
