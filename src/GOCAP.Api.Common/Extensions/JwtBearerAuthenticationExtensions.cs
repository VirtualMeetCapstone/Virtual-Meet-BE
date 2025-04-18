using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace GOCAP.Api.Common;

public static class JwtBearerAuthenticationExtensions
{
    private const string NameClaimType = "id";

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration _configuration)
    {
        var keyBytes = Convert.FromBase64String(_configuration["Jwt:SecretKey"] ?? "");
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        // Configure jwt authentication
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = NameClaimType,
                };
            })
            ;
        return services;
    }
}
