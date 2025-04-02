namespace GOCAP.Api.Controllers;

public class AuthController(IAuthService _service, IMapper _mapper) : ApiControllerBase
{
    [HttpGet("signin/google")]
    public async Task<TokenResponse> SignInGoogle([FromQuery] string idToken)
    {
        var payload = await _service.VerifyGoogleTokenAsync(idToken);
        var user = await _service.GetOrCreateUserAsync(payload);
        var jwtToken = await _service.GenerateJwtTokenAsync(user);
        return jwtToken;
    }

    [HttpPost("refresh-token")]
    public async Task<TokenResponse> RefreshTokens([FromBody] RefreshTokenModel model)
    {
        var domain = _mapper.Map<RefreshToken>(model);
        var jwtToken = await _service.RefreshTokensAsync(domain);
        return jwtToken;
    }

    [HttpPost("logout")]
    public async Task<OperationResult> Logout([FromBody] RefreshTokenModel model)
    {
        var domain = _mapper.Map<RefreshToken>(model);
        var jwtToken = await _service.LogoutAsync(domain);
        return jwtToken;
    }
}
//https://www.facebook.com/v10.0/dialog/oauth?client_id=1259288345202083&redirect_uri=https://localhost:7035/signin/facebook-response&response_type=code&scope=email,public_profile
