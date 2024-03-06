using Commerce.Domain;

namespace Commerce.Business;

public sealed class UserReadModel : ReadModel<User>
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string EmailAddress { get; set; } = string.Empty;

    public override User ToAggregate() => User.Create(Id, FirstName, LastName, EmailAddress).Value;

    public UserReadModel FromAggregate(User aggregate)
    {
        Id = aggregate.Id;
        FirstName = aggregate.FirstName;
        LastName = aggregate.LastName;
        EmailAddress = aggregate.EmailAddress;

        return this;
    }
}