using Commerce.Domain;
using CSharpFunctionalExtensions;

namespace Commerce.Business;

public sealed record Token(string Value, DateTime Expiry);

public interface ITokenService
{
    Task<Token> GenerateRefreshToken(User user);

    Result<string> GenerateAuthToken(User user);

    Task<Result<(string AuthToken, Token RefreshToken)>> Refresh(string expiredToken, string refreshToken);
}