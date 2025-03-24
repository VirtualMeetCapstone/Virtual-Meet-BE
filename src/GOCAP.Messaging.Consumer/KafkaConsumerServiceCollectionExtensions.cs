using Microsoft.Extensions.DependencyInjection;

namespace GOCAP.Messaging.Consumer;

public static class KafkaConsumerServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaConsumerServices(this IServiceCollection services)
    {
        // Register Consumer
        services.AddSingleton<UserLoginConsumer>();
        services.AddSingleton<NotificationConsumer>();
        services.AddSingleton<SearchHistoryConsumer>();
        services.AddHostedService<KafkaConsumerService>();

        return services;
    }
}
