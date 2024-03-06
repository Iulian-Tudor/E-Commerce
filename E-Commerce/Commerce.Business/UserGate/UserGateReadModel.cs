using Commerce.Domain;

namespace Commerce.Business;

public sealed class UserGateReadModel : ReadModel<UserGate>
{
    public Guid UserId { get; set; }

    public string PassCode { get; set; } = string.Empty;

    public string Secret { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? PassedAt { get; set; }

    public DateTime? ExchangedAt { get; set; }

    public override UserGate ToAggregate() => UserGate.Create(Id, UserId, PassCode, Secret, CreatedAt, PassedAt, ExchangedAt).Value;

    public UserGateReadModel FromAggregate(UserGate aggregate)
    {
        Id = aggregate.Id;
        UserId = aggregate.UserId;
        PassCode = aggregate.PassCode;
        Secret = aggregate.Secret;
        CreatedAt = aggregate.CreatedAt;
        PassedAt = aggregate.PassedAt.HasValue ? aggregate.PassedAt.Value : null;
        ExchangedAt = aggregate.ExchangedAt.HasValue ? aggregate.ExchangedAt.Value : null;

        return this;
    }
}