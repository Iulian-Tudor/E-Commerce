namespace Commerce.Business;

public interface IIdentifiedUser
{
    Guid Id { get; }

    string EmailAddress { get; }

    string FirstName { get; }

    string LastName { get; }
}