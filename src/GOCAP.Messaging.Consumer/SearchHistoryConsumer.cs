using GOCAP.Common;
using GOCAP.Domain;
using GOCAP.Services.Intention;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GOCAP.Messaging.Consumer;

public class SearchHistoryConsumer(
    IOptions<KafkaSettings> _kafkaSettings,
     ILogger<SearchHistoryConsumer> _logger,
     IServiceProvider _serviceProvider
    )
     : KafkaConsumerBase(_kafkaSettings, _logger, KafkaConstants.Topics.SearchHistory)
{
    protected override async Task ProcessMessageAsync(string message)
    {
        var searchHistory = JsonHelper.Deserialize<SearchHistory>(message);
        if (searchHistory == null)
        {
            _logger.LogWarning("[Kafka] Invalid notification event.");
            return;
        }
        using var scope = _serviceProvider.CreateScope();
        var searchHistoryService = scope.ServiceProvider.GetRequiredService<ISearchHistoryService>();
        await searchHistoryService.AddAsync(searchHistory);
        _logger.LogInformation("[Kafka] Handled search history event successfully.");
        await Task.CompletedTask;
    }
}
