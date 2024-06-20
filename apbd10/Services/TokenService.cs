using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using apbd10.Models;
using Microsoft.IdentityModel.Tokens;

namespace apbd10.Services;

public interface ITokenService
{
    public (string token, DateTime expiration) GenerateRefreshToken();
    public string GenerateAccessToken(User user);
    public ClaimsPrincipal? ValidateAndGetPrincipalFromJwt(string token, bool validateLifetime = true);
}

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly JwtSettings _jwtSettings;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        _jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();

    }

    public (string token, DateTime expiration) GenerateRefreshToken()
    {
        return (Guid.NewGuid().ToString(), DateTime.UtcNow.AddHours(24));
        
    }
    

    public string GenerateAccessToken(User user)
    {
        var userClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));

        var token = new JwtSecurityToken(
            claims: userClaims,
            expires: DateTime.UtcNow.AddHours(_jwtSettings.TokenLifetimeInHours),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? ValidateAndGetPrincipalFromJwt(string token, bool validateLifetime = true)
    {
        try
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = validateLifetime,
                ClockSkew = TimeSpan.Zero
            };
            
            ClaimsPrincipal? principal = new JwtSecurityTokenHandler()
                .ValidateToken(token, tokenValidationParameters, out var securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }    
}
public class JwtSettings
{
    public string Secret { get; set; }
    public int TokenLifetimeInHours { get; set; }
}