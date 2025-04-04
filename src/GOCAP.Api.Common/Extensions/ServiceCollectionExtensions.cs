using GOCAP.Database;
using GOCAP.ExternalServices;
using GOCAP.Messaging.Consumer;
using GOCAP.Messaging.Producer;
using GOCAP.Repository;
using GOCAP.Repository.Intention;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GOCAP.Api.Common;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure email service.
        services.AddEmailService(configuration);

        // Configure LiveKit settings.
        services.AddSingleton(sp => sp.GetRequiredService<IAppConfiguration>().GetLiveKitSettings());

        // Configure azure blob storage.
        services.AddBlobStorageService(configuration);

        // Configure kafka producer service.
        services.AddKafkaProducerServices(configuration);

        // Configure cache service.
        services.AddCacheService(configuration);

        // Configure sql server.
        services
            .AddSingleton<IAppConfiguration, AppConfiguration>()
            .AddDbContext<AppSqlDbContext>((serviceProvider, options) =>
            {
                var configuration = serviceProvider.GetRequiredService<IAppConfiguration>();
                var connectionString = configuration.GetSqlServerConnectionString();
                options.UseSqlServer(connectionString);
            });

        // Configure mongodb.
        services.AddSingleton(sp =>
        {
            var databaseName = AppConstants.DatabaseName;
            var connectionString = configuration.GetConnectionString(AppConstants.MongoDbConnection) ?? string.Empty;
            return new AppMongoDbContext(databaseName, connectionString);
        });

        // Configure unit of work.
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddServicesFromAssembly([
            Assembly.GetEntryAssembly() ?? Assembly.Load(""),
            Assembly.Load("gocap.services"),
            Assembly.Load("gocap.repository")
        ]);

        // Configure kafka consumer service.
        services.AddKafkaConsumerServices();

        return services;
    }

    public static IServiceCollection AddServicesFromAssembly(this IServiceCollection services, List<Assembly> assemblies)
    {
        assemblies?.ForEach(assembly => services.ScanTypes([.. assembly.GetTypes().Where(type => type.IsClass && !type.IsAbstract)]));
        return services;
    }

    public static IServiceCollection ScanTypes(this IServiceCollection services, List<Type> types)
    {
        foreach (var type in types)
        {
            var attribute = type.GetCustomAttribute<RegisterServiceAttribute>();

            if (attribute == null)
            {
                continue;
            }

            // Register service
            switch (attribute.Scope)
            {
                case ServiceLifetime.Transient:
                    services.AddTransient(attribute.ServiceType, type);
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped(attribute.ServiceType, type);
                    break;
                case ServiceLifetime.Singleton:
                    services.AddSingleton(attribute.ServiceType, type);
                    break;
            }
        }
        return services;
    }
}
