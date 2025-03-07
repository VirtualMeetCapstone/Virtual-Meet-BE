namespace GOCAP.Common;

public interface IAppConfiguration
{
    string? GetSqlServerConnectionString();
    string? GetGoogleClientIdString();
    JwtSettings GetJwtSettings();
    FileSettings GetFileSettings();
    string GetDefaultDomain();
}
