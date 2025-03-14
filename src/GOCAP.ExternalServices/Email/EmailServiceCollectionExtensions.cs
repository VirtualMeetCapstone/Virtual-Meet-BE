namespace GOCAP.ExternalServices;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class EmailServiceCollectionExtensions
{
    public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
    {
        var mailSettingsSection = configuration.GetSection("MailSettings");
        services.Configure<EmailSettings>(mailSettingsSection);
        services.AddTransient<IEmailService, EmailService>();

        return services;
    }
}

