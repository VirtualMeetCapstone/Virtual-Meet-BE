using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication;

namespace GOCAP.Api.Controllers;

[Route("signin")]
[ApiController]
public class AuthController(
    IConfiguration _configuration, IUserService _userService,
    IMapper _mapper, IGoogleAuthService _googleAuthService,
    IFacebookAuthService _facebookAuthService,
    ILogger<AuthController> _logger
    ) : ApiControllerBase
{
    [HttpGet("google")]
    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action("GoogleCallback", "Auth");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, "Google");
    }

    [HttpGet("facebook")]
    public IActionResult FacebookLogin()
    {
        var redirectUrl = Url.Action("FacebookCallback", "Auth");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, "Facebook");
    }

    [HttpGet("google/callback")]
    public async Task<ApiResponse> GoogleCallBack()
    {
        return await ProviderResponse("Google", ProviderType.Google);
    }

    [HttpGet("facebook/callback")]
    public async Task<ApiResponse> FacebookCallBack()
    {
        return await ProviderResponse("Facebook", ProviderType.Facebook);
    }

    private async Task<ApiResponse> ProviderResponse(string code, ProviderType providerType)
    {
        if (string.IsNullOrEmpty(code))
        {
            return new ApiResponse
            {
                Result = new OperationResult(false, "Authorization code is missing."),
                Data = null
            };
        }

        try
        {
            TokenResponse tokenResponse;
            ProviderUserBase? userInfo;

            // Handle each provider.
            if (providerType == ProviderType.Google)
            {
                tokenResponse = await _googleAuthService.ExchangeCodeForTokensAsync(code);
                userInfo = await _googleAuthService.GetUserInfoAsync(tokenResponse.AccessToken);
            }
            else if (providerType == ProviderType.Facebook)
            {
                tokenResponse = await _facebookAuthService.ExchangeCodeForTokensAsync(code);
                userInfo = await _facebookAuthService.GetUserInfoAsync(tokenResponse.AccessToken);
            }
            else
            {
                return new ApiResponse
                {
                    Result = new OperationResult(false, "Unsupported provider type."),
                    Data = null
                };
            }

            var email = userInfo?.Email;
            if (email != null)
            {
                var isEmailExists = await _userService.IsEmailExists(email);
                User? user;

                if (!isEmailExists)
                {
                    user = _mapper.Map<User>(userInfo);
                    await _userService.AddAsync(user);
                }
                else
                {
                    user = await _userService.GetByEmail(email);
                }

                return new ApiResponse
                {
                    Result = new OperationResult(true),
                    Data = GenerateJwtToken(user)
                };
            }

            return new ApiResponse
            {
                Result = new OperationResult(false, "Email was not found from the provider."),
                Data = null
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse
            {
                Result = new OperationResult(false, $"An unexpected error occurred: {ex.Message}."),
                Data = null
            };
        }
    }
    private string? GenerateJwtToken(User? user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }

        try
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"] ?? "");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity
                ([
                    new Claim(ClaimTypes.Name, user.Name ?? ""),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim("Id", user.Id.ToString() ?? ""),
                ]),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
        catch (ArgumentNullException argEx)
        {
            _logger.LogError(argEx, "An ArgumentNullException occurred: {Message}", argEx.Message);
            throw new ArgumentException("Argument null exception occurred.");
        }
        catch (SecurityTokenException secEx)
        {
            _logger.LogError(secEx, "Security token exception occurred: {Message}", secEx.Message);
            throw new SecurityTokenException("Security token exception occurred.");
        }
        catch (CryptographicException cryptoEx)
        {
            _logger.LogError(cryptoEx, "Cryptographic exception occurred: {Message}", cryptoEx.Message);
            throw new SecurityTokenException("Cryptographic exception occurred.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while generating JWT token: {Message}", ex.Message);
            throw new Exception("An unexpected error occurred while generating JWT token.");
        }
    }
}

//https://accounts.google.com/o/oauth2/auth?client_id=192713002905-t0n84r06cfn8seolr6u78os85jf276sq.apps.googleusercontent.com&redirect_uri=https://localhost:7035/signin/google-response&response_type=code&scope=openid%20email%20profile&access_type=offline&prompt=consent
//https://www.facebook.com/v10.0/dialog/oauth?client_id=1259288345202083&redirect_uri=https://localhost:7035/signin/facebook-response&response_type=code&scope=email,public_profile