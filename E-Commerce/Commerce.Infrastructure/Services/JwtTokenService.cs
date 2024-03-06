using Commerce.Domain;
using Commerce.Business;
using System.Security.Claims;
using CSharpFunctionalExtensions;
using Commerce.SharedKernel.Domain;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

using Errors = Commerce.Infrastructure.InfrastructureErrors.JwtTokenService.GenerateToken;
using TimeProvider = Commerce.SharedKernel.Domain.TimeProvider;

namespace Commerce.Infrastructure;

internal sealed class JwtTokenService(JwtConfiguration configuration, IJwtTokenRepository repository) : ITokenService
{
    public Result<string> GenerateAuthToken(User user)
    {
        var userResult = user.EnsureNotNull(Errors.UserNotProvided);

        var claimsResult = userResult
            .Map(u => new[]
            {
                new Claim(CommerceClaims.UserId, u.Id.ToString()),
                new Claim(CommerceClaims.UserEmail, u.EmailAddress),
                new Claim(CommerceClaims.UserFirstName, u.FirstName),
                new Claim(CommerceClaims.UserLastName, u.LastName),
            });

        return claimsResult.Map(GenerateToken);
    }

    public async Task<Result<(string AuthToken, Token RefreshToken)>> Refresh(string expiredToken, string refreshToken)
    {
        var tokenResult = DecryptToken(expiredToken);
        var userIdResult = tokenResult
            .Bind(t => t.Claims.TryFirst(c => c.Type == CommerceClaims.UserId).ToResult("UserId claim missing"))
            .Map(c => Guid.Parse(c.Value));

        if (userIdResult.IsFailure)
        {
            return Result.Failure<(string AuthToken, Token RefreshToken)>("Invalid jwt token");
        }

        var token = tokenResult.Value;
        var userId = userIdResult.Value;

        Maybe<RefreshTokenReadModel> existingRefreshToken = repository
            .Query()
            .FirstOrDefault(rt => rt.UserId == userId && rt.Token == refreshToken);

        var refreshResult = existingRefreshToken.ToResult("Invalid refresh token")
            .Map(rt => rt.CreatedAt + configuration.RefreshTokenLifetime)
            .Ensure(expiry => expiry > TimeProvider.Instance().UtcNow, "Invalid refresh token");

        if (refreshResult.IsFailure)
        {
            return Result.Failure<(string AuthToken, Token RefreshToken)>("Invalid token");
        }

        var dbRefreshToken = existingRefreshToken.Value;
        dbRefreshToken.Token = Guid.NewGuid().ToString();
        dbRefreshToken.CreatedAt = TimeProvider.Instance().UtcNow;

        await repository.Update(dbRefreshToken);

        var ignoredClaims = new List<string> { "nbf", "exp", "iat", "iss", "aud" };
        var claimsToCopy = token.Claims.Where(c => !ignoredClaims.Contains(c.Type));

        return (GenerateToken(claimsToCopy), new(dbRefreshToken.Token, dbRefreshToken.CreatedAt + configuration.RefreshTokenLifetime));
    }

    public async Task<Token> GenerateRefreshToken(User user)
    {
        var model = new RefreshTokenReadModel
        {
            UserId = user.Id,
            Token = Guid.NewGuid().ToString(),
            CreatedAt = TimeProvider.Instance().UtcNow
        };

        await repository.Insert(model);

        return new(model.Token, model.CreatedAt + configuration.RefreshTokenLifetime);
    }

    private Result<JwtSecurityToken> DecryptToken(string token)
    {
        var validationParams = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = configuration.Audience,
            ValidateIssuer = true,
            ValidIssuer = configuration.Issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = configuration.SigningKey,
            ValidateLifetime = false
        };

        var handler = new JwtSecurityTokenHandler();
        return Result.Try(() =>
            {
                handler.ValidateToken(token, validationParams, out var validatedToken);
                return validatedToken;
            })
            .Ensure(t => t is not null, "Invalid token")
            .Map(_ => handler.ReadJwtToken(token));
    }

    private string GenerateToken(IEnumerable<Claim> claims)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = TimeProvider.Instance().UtcNow + configuration.TokenLifetime,
            Issuer = configuration.Issuer,
            Audience = configuration.Audience,
            SigningCredentials = configuration.SigningCredentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}