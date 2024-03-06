namespace Commerce.Infrastructure;

internal sealed record SendGridConfiguration
{
    public const string SectionName = "SendGrid";

    public IReadOnlyCollection<string> DeveloperEmails { get; init; } = new List<string>();
}