using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.Authorization;

namespace Service.Security.Authorization;

public class JWTAuthorizationService(IConfiguration configuration) : IAuthorizationService
{
    private readonly string _issuer =
        configuration["Jwt:Issuer"]
        ?? throw new InvalidOperationException("JWT Issuer is missing.");

    private readonly string _audience =
        configuration["Jwt:Audience"]
        ?? throw new InvalidOperationException("JWT Audience is missing.");

    private readonly int _expirationMinutes = int.Parse(
        configuration["Jwt:ExpirationMinutes"]);



    private readonly SymmetricSecurityKey _key = new(
        Encoding.UTF8.GetBytes(
            configuration["JwtSecret"]
            ?? throw new InvalidOperationException("JwtSecret is missing.")
        ));

    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
            signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}