using GOCAP.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GOCAP.Messaging.Producer;

public static class KafkaProducerServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaProducerServices(this IServiceCollection services, IConfiguration configuration)
    {
        var kafkaSettingsSection = configuration.GetSection("KafkaSettings");
        services.Configure<KafkaSettings>(kafkaSettingsSection);

        // Register Producer
        services.AddSingleton<IKafkaProducer, KafkaProducer>();

        return services;
    }
}
