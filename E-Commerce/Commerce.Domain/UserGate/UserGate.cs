using NanoidDotNet;
using CSharpFunctionalExtensions;
using Commerce.SharedKernel.Domain;
using System.Security.Cryptography;

using Errors = Commerce.Domain.DomainErrors.UserGate;
using Constants = Commerce.Domain.DomainConstants.UserGate;
using TimeProvider = Commerce.SharedKernel.Domain.TimeProvider;

namespace Commerce.Domain;

public sealed class UserGate : AggregateRoot
{
    private UserGate(Guid userId)
    {
        UserId = userId;
        PassCode = Nanoid.Generate("0123456789", 6);
        Secret = CreateSecret();
        CreatedAt = TimeProvider.Instance().UtcNow;
        PassedAt = Maybe.None;
        ExchangedAt = Maybe.None;
    }
    
    public static Result<UserGate> Create(Guid id, Guid userId, string passCode, string secret, DateTime createdAt,DateTime? passedAt, DateTime? exchangedAt)
        => Create(userId).Tap(ug =>
        {
            ug.Id = id;
            ug.PassCode = passCode;
            ug.Secret = secret;
            ug.CreatedAt = createdAt;
            ug.PassedAt = passedAt.HasValue ? Maybe<DateTime>.From(passedAt.Value) : Maybe<DateTime>.None;
            ug.ExchangedAt = exchangedAt.HasValue ? Maybe<DateTime>.From(exchangedAt.Value) : Maybe<DateTime>.None;
        });

    public static Result<UserGate> Create(Guid userId)
    {
        return Result
            .FailureIf(userId == Guid.Empty, Errors.Create.UserIdEmpty)
            .Map(() => new UserGate(userId));
    }

    public Guid UserId { get; private set; }

    public string PassCode { get; private set; }

    public string Secret { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public Maybe<DateTime> PassedAt { get; private set; }

    public Maybe<DateTime> ExchangedAt { get; private set; }

    public DateTime ClosesAt => CreatedAt.AddMinutes(Constants.LifetimeInMinutes);

    public Maybe<DateTime> ExchangeWindowExpiresAt => PassedAt.Map(p => p.AddMinutes(Constants.ExchangeWindow));

    public Result Pass(string passCode)
    {
        return Result
            .FailureIf(PassedAt.HasValue, Errors.Pass.UserGateAlreadyPassed)
            .Ensure(() => ClosesAt > TimeProvider.Instance().UtcNow, Errors.Pass.UserGateExpired)
            .Ensure(() => PassCode == passCode, Errors.Pass.PassCodeInvalid)
            .Tap(() => PassedAt = TimeProvider.Instance().UtcNow);
    }

    public Result Exchange(string secret)
    {
        return Result
            .FailureIf(ExchangedAt.HasValue, Errors.Exchange.UserGateAlreadyExchanged)
            .Ensure(() => PassedAt.HasValue, Errors.Exchange.UserGateNotPassed)
            .Ensure(() => ExchangeWindowExpiresAt.Value > TimeProvider.Instance().UtcNow, Errors.Exchange.UserGateExpired)
            .Ensure(() => Secret == secret, Errors.Exchange.SecretInvalid)
            .Tap(() => ExchangedAt = TimeProvider.Instance().UtcNow);
    }

    private static string CreateSecret()
    {
        var key = new byte[32];
        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(key);

        return Convert.ToBase64String(key);
    }
}

