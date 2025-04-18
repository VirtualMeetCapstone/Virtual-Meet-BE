using Nest;

namespace GOCAP.ExternalServices;

public static class ElasticServiceCollectionExtensions
{
    public static IServiceCollection AddElasticService(this IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.GetSection("ElasticSearch");
        var uri = new Uri(config["Uri"] ?? "");

        var settings = new ConnectionSettings(uri)
            .DefaultIndex($"{config["IndexPrefix"]}-default")
            .EnableDebugMode();

        var client = new ElasticClient(settings);

        services.AddSingleton<IElasticClient>(client);

        return services;
    }
}

