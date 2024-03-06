using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Commerce.Infrastructure;

internal sealed record JwtConfiguration
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; }

    public string Audience { get; init; }

    public string Key { get; init; }

    public int TokenLifetimeMinutes { get; init; }

    public int RefreshTokenLifetimeMinutes { get; init; }

    public TimeSpan TokenLifetime => TimeSpan.FromMinutes(TokenLifetimeMinutes);

    public TimeSpan RefreshTokenLifetime => TimeSpan.FromMinutes(RefreshTokenLifetimeMinutes);

    public SymmetricSecurityKey SigningKey => new(Encoding.UTF8.GetBytes(Key));

    public SigningCredentials SigningCredentials => new(SigningKey, SecurityAlgorithms.HmacSha256Signature);
}