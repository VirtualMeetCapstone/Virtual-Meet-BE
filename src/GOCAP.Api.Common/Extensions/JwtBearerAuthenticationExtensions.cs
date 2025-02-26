using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GOCAP.Api.Common;

public static class JwtBearerAuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration _configuration)
    {
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"] ?? "");

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
                    ValidIssuer = AppConstants.DefaultUri,
                    ValidAudience = AppConstants.DefaultUri,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            })
            ;
        return services;
    }
}
