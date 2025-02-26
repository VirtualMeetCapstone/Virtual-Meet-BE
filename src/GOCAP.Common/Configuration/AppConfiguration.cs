using Microsoft.Extensions.Configuration;

namespace GOCAP.Common;

public class AppConfiguration(IConfiguration _configuration) : IAppConfiguration
{

    /// <summary>
    /// Get sql server connection string.
    /// </summary>
    /// <returns></returns>
    public string GetSqlServerConnectionString()
    {
        return _configuration.GetConnectionString(AppConstants.SqlServerConnection)
            ?? throw new InternalException();
    }   

    /// <summary>
    /// Get google client id string.
    /// </summary>
    /// <returns></returns>
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
        jwtSettings.Issuer = GetDefaultDomain();
        jwtSettings.Audience = GetDefaultDomain();
        return jwtSettings;
    }

    public string GetDefaultDomain()
    => _configuration.GetSection("Domain").Value ?? "";
}