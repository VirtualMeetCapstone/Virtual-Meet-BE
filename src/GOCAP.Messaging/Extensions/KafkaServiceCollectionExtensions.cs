using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GOCAP.Messaging;

public static class KafkaServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaServices(this IServiceCollection services, IConfiguration configuration)
    {
        var kafkaSettingsSection = configuration.GetSection("KafkaSettings");
        services.Configure<KafkaSettings>(kafkaSettingsSection);

        // Register Producer and Consumer
        services.AddSingleton<IKafkaProducer, KafkaProducer>();
        services.AddSingleton<UserLoginConsumer>();
        services.AddHostedService<KafkaConsumerService>();

        return services;
    }
}
