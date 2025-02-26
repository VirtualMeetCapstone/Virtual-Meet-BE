using Google.Apis.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GOCAP.Services;

[RegisterService(typeof(IAuthService))]
internal class AuthService(IAppConfiguration _appConfiguration,
    IUserRepository _userRepository, IMapper _mapper) : IAuthService
{
    public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleTokenAsync(string idToken)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = [_appConfiguration.GetGoogleClientIdString()]
        };

        return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
    }
    public async Task<User> GetOrCreateUserAsync(GoogleJsonWebSignature.Payload payload)
    {
        var user = await _userRepository.GetByEmailAsync(payload.Email);

        if (user == null)
        {
            user = new UserEntity
            {
                Email = payload.Email,
                Name = payload.Name,
                Picture = payload.Picture,
            };
            user.InitCreation();
            await _userRepository.AddAsync(user);
        }
        var result = _mapper.Map<User>(user);
        return result;
    }
    public string GenerateJwtToken(User user)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var jwtSettings = _appConfiguration.GetJwtSettings();
        var secretKeyBytes = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim("Picture", JsonHelper.Serialize(user.Picture)),
                new Claim(ClaimTypes.Name, user.Name ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim("Id", user.Id.ToString() ?? ""),
            ]),
            Expires = DateTime.UtcNow.AddMinutes(jwtSettings.AccessTokenExpiration),
            Issuer = jwtSettings.Issuer,
            Audience = jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature)
        };

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        return jwtTokenHandler.WriteToken(token);
    }
}
