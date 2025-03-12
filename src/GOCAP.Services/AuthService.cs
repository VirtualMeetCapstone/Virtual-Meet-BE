using GOCAP.Messaging.Producer;
using Google.Apis.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace GOCAP.Services;

[RegisterService(typeof(IAuthService))]
internal class AuthService(IAppConfiguration _appConfiguration,
    IUserRoleRepository _userRoleRepository,
    IKafkaProducer _kafkaProducer,
    IRoleRepository _roleRepository,
    IUnitOfWork _unitOfWork,
    IUserRepository _userRepository,
    IMapper _mapper,
    ILogger<AuthService> _logger) : IAuthService
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
                Id = Guid.NewGuid(),
                Email = payload.Email,
                Name = payload.Name,
                Picture = JsonHelper.Serialize(new Media
                {
                    Url = payload.Picture,
                }) ?? "",
            };
            user.InitCreation();
            // Get default role (user).
            var defaultRole = await _roleRepository.GetRoleByNameAsync(RoleName.User);
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var newUser = await _userRepository.AddAsync(user); // Add new user
                if (defaultRole != null)
                {
                    // Assgin role to new user.
                    await _userRoleRepository.AssignRoleToUserAsync(new UserRoleEntity
                    {
                        RoleId = defaultRole.Id,
                        UserId = newUser.Id,
                    });
                }
                await _unitOfWork.CommitTransactionAsync();
                await _kafkaProducer.ProduceAsync(KafkaConstants.Topics.UserLogin, 
                    new UserLoginEvent{
                        Email = newUser.Email,
                        Username = newUser.Name,
                        LoginTime = newUser.CreateTime
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
                await _unitOfWork.RollbackTransactionAsync();
                throw new InternalException();
            }
        }
        var result = _mapper.Map<User>(user);
        return result;
    }
    public async Task<string> GenerateJwtTokenAsync(User user)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var jwtSettings = _appConfiguration.GetJwtSettings();
        var secretKeyBytes = Convert.FromBase64String(jwtSettings.SecretKey);

        // Get role list of user.
        var roles = await _roleRepository.GetRolesByUserIdAsync(user.Id);
        var defaultRole = roles.FirstOrDefault();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim("picture", JsonHelper.Serialize(user.Picture) ?? ""),
                new Claim(ClaimTypes.Name, user.Name ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim("id", user.Id.ToString() ?? ""),
                new Claim(ClaimTypes.Role, defaultRole?.Name ?? "")
            ]),
            Expires = DateTime.UtcNow.AddMinutes(jwtSettings.AccessTokenExpiration),
            Issuer = jwtSettings.Issuer,
            Audience = jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature)
        };

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        return jwtTokenHandler.WriteToken(token);
    }

    public string GenerateRefreshTokenAsync(Guid userId)
    {
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var tokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7).Ticks
        };
        // Save to db
        return refreshToken;
    }
}
