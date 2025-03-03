namespace GOCAP.ExternalServices;

using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class BlobStorageServiceCollectionExtensions
{
    public static IServiceCollection AddBlobStorageService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new BlobServiceClient(configuration.GetConnectionString(AppConstants.AzureBlobStorage)));
        services.AddSingleton<IBlobStorageService, BlobStorageService>();

        return services;
    }
}

