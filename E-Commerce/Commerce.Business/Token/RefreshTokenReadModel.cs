namespace Commerce.Business;

public sealed class RefreshTokenReadModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }

    public string Token { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}