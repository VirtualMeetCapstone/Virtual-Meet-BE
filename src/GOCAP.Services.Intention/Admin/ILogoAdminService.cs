namespace GOCAP.Services.Intention;

public interface ILogoAdminService
{
    Task<LogoUpdate> UpdateAsync(LogoUpdate domain);
    Task<LogoUpdate> GetAsync();
}
