namespace GOCAP.Services.Intention;

public interface ILogoService
{
    Task<OperationResult> CreateOrUpdateAsync(Logo domain);
    Task<Logo> GetAsync();
}
