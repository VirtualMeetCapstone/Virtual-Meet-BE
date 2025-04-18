using Google.Apis.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GOCAP.Services;

[RegisterService(typeof(IAuthService))]
internal class AuthService(
    IAppConfiguration _appConfiguration,
    IUserService _userService,
    IRedisService _redisService,
    IUserRoleRepository _userRoleRepository,
    IKafkaProducer _kafkaProducer,
    IRoleRepository _roleRepository,
    IUnitOfWork _unitOfWork,
    IUserRepository _userRepository,
    IMapper _mapper,
    ILogger<AuthService> _logger) : IAuthService
{
    private static readonly string RefreshTokenPrefix = "RefreshToken:";
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
                _ = _kafkaProducer.ProduceAsync(KafkaConstants.Topics.UserLogin,
                        new UserLoginEvent
                        {
                            Email = newUser.Email,
                            Username = newUser.Name,
                            LoginTime = newUser.CreateTime
                        });

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
    public async Task<TokenResponse> GenerateJwtTokenAsync(User user)
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
        var accessToken = jwtTokenHandler.WriteToken(token);
        var refreshToken = await GenerateRefreshTokenAsync(user.Id);
        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    private async Task<string> GenerateRefreshTokenAsync(Guid userId)
    {
        // Generate a raw refresh token (64 bytes)
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        // Hash the refresh token using SHA-256 for secure storage
        var hashedRefreshToken = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));

        // Determine the expiration time for the refresh token
        var refreshTokenExpirationMinutes = _appConfiguration.GetJwtSettings().RefreshTokenExpiration;
        var expiresAt = DateTime.UtcNow.AddMinutes(refreshTokenExpirationMinutes);
        var ttl = expiresAt - DateTime.UtcNow;

        // Store the hashed refresh token in Redis with the associated userId
        var redisKey = $"{RefreshTokenPrefix}{hashedRefreshToken}";
        await _redisService.SetAsync(redisKey, userId, ttl);

        // Return the raw refresh token to the client
        return refreshToken;
    }

    public async Task<TokenResponse> RefreshTokensAsync(RefreshToken domain)
    {
        // Hash the received refresh token using SHA-256 to match stored keys
        var hashedRefreshToken = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(domain.Token)));

        // Retrieve the associated user ID from Redis
        var redisKey = $"{RefreshTokenPrefix}{hashedRefreshToken}";
        var userId = await _redisService.GetAsync<Guid>(redisKey);

        // If the user ID is not found, the refresh token is invalid
        if (userId == Guid.Empty)
        {
            throw new AuthenticationFailedException("Invalid refresh token.");
        }

        // Retrieve the user from the database
        var user = await _userService.GetByIdAsync(userId) ?? throw new ResourceNotFoundException("User not found.");

        // Generate a new access token and refresh token
        var tokenResponse = await GenerateJwtTokenAsync(user);

        return tokenResponse;
    }

    public async Task<OperationResult> LogoutAsync(RefreshToken domain)
    {
        if (string.IsNullOrEmpty(domain.Token))
        {
            throw new ParameterInvalidException("Refresh token is required.");
        }

        // Hash the refresh token to match stored keys
        var hashedRefreshToken = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(domain.Token)));

        // Remove refresh token from Redis
        var redisKey = $"{RefreshTokenPrefix}{hashedRefreshToken}";
        var userId = await _redisService.GetAsync<Guid>(redisKey);

        if (userId != Guid.Empty)
        {
            await _redisService.DeleteAsync(redisKey);
        }
        return new OperationResult(true);
    }
}
