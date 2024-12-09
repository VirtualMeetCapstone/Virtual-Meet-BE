using Microsoft.Extensions.Configuration;

namespace GOCAP.Common;

public class GOCAPConfiguration
{
    private static GOCAPConfiguration? _instance;
    private static readonly object _lock = new();
    private static IConfiguration? _configuration;

    // Private constructor để ngăn khởi tạo từ bên ngoài
    private GOCAPConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Static method để lấy instance duy nhất
    public static GOCAPConfiguration GetInstance(IConfiguration configuration)
    {
        // Dùng lock để đảm bảo thread-safe
        lock (_lock)
        {
            _instance ??= new GOCAPConfiguration(configuration);
        }

        return _instance;
    }

    public static void Initialize(IConfiguration configuration)
    {
        _configuration ??= configuration;
    }

    // Get sql server connection string
    public static string GetSqlServerConnectionString()
    {
        if (_configuration == null)
        {
            throw new InvalidOperationException("Configuration has not been initialized.");
        }

        return _configuration.GetConnectionString("SqlServerConnection") ?? string.Empty;
    }   
}