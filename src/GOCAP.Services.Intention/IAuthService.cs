using Google.Apis.Auth;

namespace GOCAP.Services.Intention;

public interface IAuthService
{
    Task<GoogleJsonWebSignature.Payload> VerifyGoogleTokenAsync(string idToken);
    Task<User> GetOrCreateUserAsync(GoogleJsonWebSignature.Payload payload);
    Task<TokenResponse> GenerateJwtTokenAsync(User user);
    Task<TokenResponse> RefreshTokensAsync(RefreshToken domain);
    Task<OperationResult> LogoutAsync(RefreshToken domain);
}
