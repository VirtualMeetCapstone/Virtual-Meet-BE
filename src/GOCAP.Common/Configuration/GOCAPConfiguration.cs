using Microsoft.Extensions.Configuration;

namespace GOCAP.Common;

public class GOCAPConfiguration (IConfiguration _configuration) : IGOCAPConfiguration
{

    /// <summary>
    /// Get sql server connection string 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public string? GetSqlServerConnectionString()
    {
        if (_configuration is null)
        {
            throw new InternalException("Configuration has not been initialized.");
        }

        return _configuration.GetConnectionString(GOCAPConstants.SqlServerConnection);
    }   
}