using Google.Apis.Auth;

namespace GOCAP.Services.Intention;

public interface IAuthService
{
    Task<GoogleJsonWebSignature.Payload> VerifyGoogleTokenAsync(string idToken);
    Task<User> GetOrCreateUserAsync(GoogleJsonWebSignature.Payload payload);
    string GenerateJwtToken(User user);
}
