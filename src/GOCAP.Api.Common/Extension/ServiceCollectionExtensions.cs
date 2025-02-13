using Azure.Storage;
using Azure.Storage.Blobs;
using GOCAP.Common;
using GOCAP.Database;
using GOCAP.Repository;
using GOCAP.Repository.Intention;
using GOCAP.Services.BlobStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Reflection;

namespace GOCAP.Api.Common;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new BlobServiceClient(configuration.GetConnectionString(GOCAPConstants.AzureBlobStorage)));
        services.AddSingleton<IBlobStorageService, BlobStorageService>();

        // Add for using sql server
        services
            .AddSingleton<IGOCAPConfiguration, GOCAPConfiguration>()
            .AddDbContext<AppSqlDbContext>((serviceProvider, options) =>
            {
                var configuration = serviceProvider.GetRequiredService<IGOCAPConfiguration>();
                var connectionString = configuration.GetSqlServerConnectionString();
                options.UseSqlServer(connectionString);
            });
        
        // Add for using MongoDB
        services.AddSingleton(sp =>
        {
            var databaseName = GOCAPConstants.DatabaseName;
            var connectionString = configuration.GetConnectionString(GOCAPConstants.MongoDbConnection) ?? string.Empty;
            return new AppMongoDbContext(databaseName, connectionString);
        });

        // Add for using unit of work pattern
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Add for using Redis
        //services.AddDistributedMemoryCache();
        //services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configuration.GetConnectionString(GOCAPConstants.RedisConnection) ?? string.Empty));

        services.AddServicesFromAssembly([
            Assembly.GetEntryAssembly() ?? Assembly.Load(""),
            Assembly.Load("gocap.services"),
            Assembly.Load("gocap.repository")
        ]);
        return services;
    }

    public static IServiceCollection AddServicesFromAssembly(this IServiceCollection services, List<Assembly> assemblies)
    {
        assemblies?.ForEach(assembly => services.ScanTypes(assembly.GetTypes().Where(type => type.IsClass && !type.IsAbstract).ToList()));
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
