using GOCAP.Database;
using GOCAP.ExternalServices;
using GOCAP.Messaging.Consumer;
using GOCAP.Messaging.Producer;
using GOCAP.Repository;
using GOCAP.Repository.Intention;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Net.payOS;
using System.Net.Http.Headers;
using System.Reflection;

namespace GOCAP.Api.Common;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure email service.
        services.AddEmailService(configuration);

        //add export excel
        services.AddScoped<IExcelExportService, ExcelExportService>();

        // Configure LiveKit settings.
        services.AddSingleton(sp => sp.GetRequiredService<IAppConfiguration>().GetLiveKitSettings());

        // Configure OpenAI_MODE1 settings.
        services.AddSingleton(sp => sp.GetRequiredService<IAppConfiguration>().GetModerationSettings());


        // Configure OpenAI_MODE2 settings.
        services.AddSingleton(sp => sp.GetRequiredService<IAppConfiguration>().GetOpenAISettings());

        // Configure Youtube settings.
        services.AddSingleton(sp => sp.GetRequiredService<IAppConfiguration>().GetYoutubeSettings());

        // Configure PayOS settings.
        services.AddSingleton(sp => sp.GetRequiredService<IAppConfiguration>().GetPayOsSettings());

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

        services.AddSingleton<PayOS>(sp =>
        {
            var settings = sp.GetRequiredService<IAppConfiguration>().GetPayOsSettings();
            return new PayOS(settings.ClientId, settings.ApiKey, settings.CheckSumKey);
        });

        services.AddScoped<IVipPaymentService, VipPaymentService>();
        services.AddServicesFromAssembly([
            Assembly.GetEntryAssembly() ?? Assembly.Load(""),
            Assembly.Load("gocap.services"),
            Assembly.Load("gocap.repository")
        ]);

        // Configure kafka consumer service.
        services.AddKafkaConsumerServices();

        //Add HttpClient for OpenAI
        services.AddHttpClient("OpenAI", (sp, client) =>
        {
            var openAISettings = sp.GetRequiredService<IAppConfiguration>().GetOpenAISettings();
            client.BaseAddress = new Uri(openAISettings.OpenAIUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", openAISettings.OpenAIKey);
        });

        //Add HttpClient for RapidAPI
        services.AddHttpClient("RapidAPI", (sp, client) =>
        {
            var rapidApiSettings = sp.GetRequiredService<IAppConfiguration>().GetModerationSettings();
            client.BaseAddress = new Uri(rapidApiSettings.ApiUrl);
            client.DefaultRequestHeaders.Add("X-RapidAPI-Key", rapidApiSettings.ApiKey);
            client.DefaultRequestHeaders.Add("X-RapidAPI-Host", rapidApiSettings.ApiHost);
        });

        services.AddScoped<IModerationRepository, ModerationRepository>();
        services.AddScoped<IAIChatRepository, AIChatRepository>();

        services.AddScoped<IAIService>(provider =>
         {
             var moderationRepo = provider.GetRequiredService<IModerationRepository>();
             var chatRepo = provider.GetRequiredService<IAIChatRepository>();
             return new AIService(moderationRepo, chatRepo, ModerationModel.RapidAPI); // hoặc OpenAI
         });
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
