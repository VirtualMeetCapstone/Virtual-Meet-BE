namespace GOCAP.Api.Controllers;

[Route("signin")]
public class AuthController(IAuthService _service) : ApiControllerBase
{
    [HttpPost("google")]
    public async Task<TokenResponse> SignInGoogle([FromQuery] string idToken)
    {
        var payload = await _service.VerifyGoogleTokenAsync(idToken);
        var user = await _service.GetOrCreateUserAsync(payload);
        var jwtToken = await _service.GenerateJwtTokenAsync(user);
        var refreshToken = _service.GenerateRefreshTokenAsync(user.Id);
        return new TokenResponse
        {
            AccessToken = jwtToken,
            RefreshToken = refreshToken,
        };
    }

}
//https://www.facebook.com/v10.0/dialog/oauth?client_id=1259288345202083&redirect_uri=https://localhost:7035/signin/facebook-response&response_type=code&scope=email,public_profile
