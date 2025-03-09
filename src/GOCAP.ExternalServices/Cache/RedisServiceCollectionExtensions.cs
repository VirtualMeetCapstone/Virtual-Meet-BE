namespace GOCAP.ExternalServices;

public static class RedisServiceCollectionExtensions
{
    public static IServiceCollection AddCacheService(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString(AppConstants.RedisConnection)
            ?? throw new ParameterInvalidException();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = AppConstants.DatabaseName;
        });

        services.AddSingleton<IConnectionMultiplexer>(sp => LazyConnection.Value);
        services.AddScoped<IRedisService, RedisService>();

        return services;
    }

    private static readonly Lazy<ConnectionMultiplexer> LazyConnection =
        new(() => ConnectionMultiplexer.Connect(AppConstants.RedisConnection));
}

