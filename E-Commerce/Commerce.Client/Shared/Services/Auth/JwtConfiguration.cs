using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Commerce.Client;

public class JwtConfiguration
{
    public string Issuer { get; } = "Commerce"; 

    public string Audience { get; } = "Commerce";

    public string Key { get; } = "helloThisIsAKeyWhichIsSuperDuperSecure";

    public int TokenLifetimeMinutes { get; } = 1440;

    public int RefreshTokenLifetimeMinutes { get; } = 1440;

    public TimeSpan TokenLifetime => TimeSpan.FromMinutes(TokenLifetimeMinutes);

    public TimeSpan RefreshTokenLifetime => TimeSpan.FromMinutes(RefreshTokenLifetimeMinutes);

    public SymmetricSecurityKey SigningKey => new(Encoding.UTF8.GetBytes(Key));

    public SigningCredentials SigningCredentials => new(SigningKey, SecurityAlgorithms.HmacSha256Signature);
}