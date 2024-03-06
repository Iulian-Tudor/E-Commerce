using EmailValidation;
using CSharpFunctionalExtensions;
using Commerce.SharedKernel.Domain;

using Errors = Commerce.Domain.DomainErrors.User;
using Constants = Commerce.Domain.DomainConstants.User;

namespace Commerce.Domain;

public sealed class User : AggregateRoot
{
    private User(string firstName, string lastName, string emailAddress)
    {
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
    }

    public static Result<User> Create(Guid id, string firstName, string lastName, string emailAddress)
        => Create(firstName, lastName, emailAddress).Tap(u => u.Id = id);

    public static Result<User> Create(string firstName, string lastName, string emailAddress)
    {
        var firstNameResult = firstName
            .EnsureNotNullOrEmpty(Errors.Create.FirstNameNullOrEmpty)
            .Ensure(f => f.Length <= Constants.FirstNameMaxLength, Errors.Create.FirstNameLongerThanMaxLength);

        var lastNameResult = lastName
            .EnsureNotNullOrEmpty(Errors.Create.LastNameNullOrEmpty)
            .Ensure(l => l.Length <= Constants.LastNameMaxLength, Errors.Create.LastNameLongerThanMaxLength);

        var emailAddressResult = emailAddress
            .EnsureNotNullOrEmpty(Errors.Create.EmailAddressNullOrEmpty)
            .Ensure(e => e.Length <= Constants.EmailAddressMaxLength, Errors.Create.EmailAddressLongerThanMaxLength)
            .Ensure(e => EmailValidator.Validate(e), Errors.Create.EmailAddressInvalidFormat);

        return Result
            .FirstFailureOrSuccess(firstNameResult, lastNameResult, emailAddressResult)
            .Map(() => new User(firstName, lastName, emailAddress));
    }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string EmailAddress { get; private set; }

    public Result ChangeDetails(string firstName, string lastName)
    {
        var firstNameResult = firstName
            .EnsureNotNullOrEmpty(Errors.ChangeDetails.FirstNameNullOrEmpty)
            .Ensure(f => f.Length <= Constants.FirstNameMaxLength, Errors.ChangeDetails.FirstNameLongerThanMaxLength);

        var lastNameResult = lastName
            .EnsureNotNullOrEmpty(Errors.ChangeDetails.LastNameNullOrEmpty)
            .Ensure(l => l.Length <= Constants.LastNameMaxLength, Errors.ChangeDetails.LastNameLongerThanMaxLength);

        return Result
            .FirstFailureOrSuccess(firstNameResult, lastNameResult)
            .Tap(() =>
            {
                FirstName = firstName;
                LastName = lastName;
            });
    }
}