namespace Commerce.Business.Tests;

public sealed record TestableUser(Guid Id, string EmailAddress, string FirstName, string LastName) : IIdentifiedUser
{
    public static TestableUser Any() => WithId(Guid.NewGuid());

    public static TestableUser WithId(Guid id) => new(id, "tudor.tescu@hotmail.com", "Tudor", "Tescu");
}