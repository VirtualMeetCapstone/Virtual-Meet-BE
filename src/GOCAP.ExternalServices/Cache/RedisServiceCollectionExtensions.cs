namespace GOCAP.ExternalServices;

public static class RedisServiceCollectionExtensions
{
    public static IServiceCollection AddCacheService(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString(AppConstants.RedisConnection)
            ?? throw new ParameterInvalidException();

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisConnectionString));
        services.AddScoped<IRedisService, RedisService>();

        return services;
    }
}

