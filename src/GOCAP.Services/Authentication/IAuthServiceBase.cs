namespace GOCAP.Services;

public interface IAuthServiceBase<T> where T : class
{
    Task<TokenResponse> ExchangeCodeForTokensAsync(string code);
    Task<T?> GetUserInfoAsync(string accessToken);
}