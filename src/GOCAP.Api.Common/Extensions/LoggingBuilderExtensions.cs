using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GOCAP.Api.Common;

public static class LoggingBuilderExtensions
{
    public static ILoggingBuilder ConfigureLogging(this ILoggingBuilder loggingBuilder, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
        loggingBuilder.AddSerilog(dispose: true);
        return loggingBuilder;
    }
}
