using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace GOCAP.Common;

public class AppConfiguration(IConfiguration _configuration, IHostEnvironment _hostEnvironment) : IAppConfiguration
{

    /// <summary>
    /// Get sql server connection string.
    /// </summary>
    /// <returns>string</returns>
    public string GetSqlServerConnectionString()
    {
        return _configuration.GetConnectionString(AppConstants.SqlServerConnection)
            ?? throw new InternalException();
    }

    /// <summary>
    /// Get google client id string.
    /// </summary>
    /// <returns>string</returns>
    public string? GetGoogleClientIdString()
    {
        return _configuration["Authentication:Google:ClientId"];
    }

    /// <summary>
    /// Get jwt settings.
    /// </summary>
    /// <returns></returns>
    public JwtSettings GetJwtSettings()
    {
        var jwtSettings = new JwtSettings();
        _configuration.GetSection("Jwt").Bind(jwtSettings);
        return jwtSettings;
    }

    /// <summary>
    /// Get file settings.
    /// </summary>
    /// <returns></returns>
    public FileSettings GetFileSettings()
    {
        var fileSettings = new FileSettings();
        _configuration.GetSection("FileSettings").Bind(fileSettings);
        return fileSettings;
    }

    /// <summary>
    /// Get defaut domain.
    /// </summary>
    /// <returns>string</returns>
    public string GetDefaultDomain()
    => _configuration.GetSection("Domain").Value ?? "";

    /// <summary>
    /// Get LiveKit settings.
    /// </summary>
    public LiveKitSettings GetLiveKitSettings()
    {
        var liveKitSettings = new LiveKitSettings();
        _configuration.GetSection("LiveKit").Bind(liveKitSettings);
        return liveKitSettings;
    }

    public ModerationSettings GetModerationSettings()
    {
        var moderationSettings = new ModerationSettings();
        _configuration.GetSection("Moderation").Bind(moderationSettings);
        return moderationSettings;
    }

    public OpenAISettings GetOpenAISettings()
    {
        var openAISettings = new OpenAISettings();
        _configuration.GetSection("OpenAI").Bind(openAISettings);
        return openAISettings;
    }
    /// <summary>
    /// Get Youtube settings.
    /// </summary>
    public YoutubeSettings GetYoutubeSettings()
    {
        var ytb = new YoutubeSettings();
        _configuration.GetSection("Youtube").Bind(ytb);
        return ytb;
    }

    /// <summary>
    /// Get PayOS settings.
    /// </summary>
    public PayOsSettings GetPayOsSettings()
    {
        var payOS = new PayOsSettings();
        _configuration.GetSection("PAYOS").Bind(payOS);
        return payOS;
    }

    /// <summary>
    /// Get current enviroment.
    /// </summary>
    /// <returns></returns>
    public string? GetEnvironment()
    {
        return _hostEnvironment.EnvironmentName;
    }
}