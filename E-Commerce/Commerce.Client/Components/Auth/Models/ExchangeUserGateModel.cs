namespace Commerce.Client;

public sealed class ExchangeUserGateModel
{
    public Guid UserId { get; set; }

    public string GateSecret { get; set; }
}

public sealed class ExchangeUserGateResponse
{
    public string AuthToken { get; set; }

    public RefreshToken RefreshToken { get; set; }
}

public sealed class RefreshToken
{
    public Guid Value { get; set; }

    public DateTime Expiry { get; set; }
}