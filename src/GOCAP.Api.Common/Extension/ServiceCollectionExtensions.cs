using Azure.Storage.Blobs;
using GOCAP.Common;
using GOCAP.Database;
using GOCAP.Services.BlobStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GOCAP.Api.Common;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new BlobServiceClient(configuration.GetConnectionString("AzureBlobStorage")));
        services.AddSingleton<IBlobStorageService, BlobStorageService>();
        services.AddDbContext<GoCapMsSqlDbContext>(options =>
                 options.UseSqlServer(configuration.GetConnectionString("GoCapSqlServerConnection")));
        services.AddSingleton(sp =>
        {
            var databaseName = "GOCAP";
            var connectionString = configuration.GetConnectionString("GoCapMongoDbConnection") ?? "";
            return new GoCapMongoDbContext(databaseName, connectionString);
        });

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
