using GOCAP.Services;
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
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            })
            .AddGoogle(options =>
            {
                options.ClientId = _configuration["Authentication:Google:ClientId"] ?? "";
                options.ClientSecret = _configuration["Authentication:Google:ClientSecret"] ?? "";
                options.CallbackPath = _configuration["Authentication:Google:RedirectUri"] ?? "";
            })
            .AddFacebook(options =>
            {
                options.AppId = _configuration["Authentication:Facebook:AppId"] ?? "";
                options.AppSecret = _configuration["Authentication:Facebook:AppSecret"] ?? "";
                options.CallbackPath = _configuration["Authentication:Facebook:RedirectUri"] ?? "";
            })
            ;
        services.AddHttpClient<IGoogleAuthService, GoogleAuthService>();
        services.AddHttpClient<IFacebookAuthService, FacebookAuthService>();
        return services;
    }
}
