using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Service;
using Service.Authorization;
using Service.Security;
using Service.Security.Authorization;

namespace Api;

public static class Startup
{
    public static void ConfigureDomainServices(
        this IServiceCollection services,IConfiguration configuration)
        
    {
        services.AddScoped<UserService>();
        services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();
        services.AddScoped<IAuthorizationService, JWTAuthorizationService>();
        
        var jwtSecret = configuration["JwtSecret"]
                        ?? throw new InvalidOperationException("JwtSecret is missing.");
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
                };
            });
        services.AddAuthorization();

    }
    
}