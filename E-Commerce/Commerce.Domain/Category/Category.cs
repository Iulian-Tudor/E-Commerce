using CSharpFunctionalExtensions;
using Commerce.SharedKernel.Domain;

using Errors = Commerce.Domain.DomainErrors.Category;
using Constants = Commerce.Domain.DomainConstants.Category;

namespace Commerce.Domain;

public sealed class Category : AggregateRoot
{
    private Category(string name, string description)
    {
        
        Name = name;
        Description = description;
    }

    public static Result<Category> Create(Guid id, string name, string description)
        => Create(name, description).Tap(c => c.Id = id);

    public static Result<Category> Create(string name, string description)
    {
        var nameResult = name
            .EnsureNotNullOrEmpty(Errors.Create.NameNullOrEmpty)
            .Ensure(n => n.Length <= Constants.NameMaxLength, Errors.Create.NameLongerThanMaxLength)
            .Ensure(n => n.Length >= Constants.NameMinLength, Errors.Create.NameShorterThanMinLength);

        var descriptionResult = description
            .EnsureNotNullOrEmpty(Errors.Create.DescriptionNullOrEmpty)
            .Ensure(d => d.Length <= Constants.DescriptionMaxLength, Errors.Create.DescriptionLongerThanMaxLength)
            .Ensure(d => d.Length >= Constants.DescriptionMinLength, Errors.Create.DescriptionShorterThanMinLength);

        return Result
            .FirstFailureOrSuccess(nameResult, descriptionResult)
            .Map(() => new Category(name, description));
    }

    public string Name { get; private set; }

    public string Description { get; private set; }
}