namespace Commerce.Client;

public sealed class UserModel
{
    public Guid Id { get; set; } = Guid.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string EmailAddress { get; set; } = string.Empty;

    public string Initials => $"{FirstName[0]}{LastName[0]}";

    public string FullName => $"{FirstName} {LastName}";
}