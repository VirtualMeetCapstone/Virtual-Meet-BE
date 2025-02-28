using Google.Apis.Auth;

namespace GOCAP.Services.Intention;

public interface IAuthService
{
    Task<GoogleJsonWebSignature.Payload> VerifyGoogleTokenAsync(string idToken);
    Task<User> GetOrCreateUserAsync(GoogleJsonWebSignature.Payload payload);
    Task<string> GenerateJwtTokenAsync(User user);
    string GenerateRefreshTokenAsync(Guid userId);
}
