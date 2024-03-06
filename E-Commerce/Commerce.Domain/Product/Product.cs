using CSharpFunctionalExtensions;
using Commerce.SharedKernel.Domain;

using Errors = Commerce.Domain.DomainErrors.Product;
using Constants = Commerce.Domain.DomainConstants.Product;

namespace Commerce.Domain;

public sealed class Product : AggregateRoot 
{
    private Product(Guid vendorId, string vendorName, string name, string description, FixedPrecisionPrice price, Guid categoryId)
    {
        VendorId = vendorId;
        VendorName = vendorName;
        Name = name;
        Description = description;
        Price = price;
        IsVisible = false;
        CategoryId = categoryId;
    }

    public static Result<Product> Create(Guid id, Guid vendorId, string vendorName, string name, string description, decimal price, Guid categoryId, bool isVisible, MediaAsset? mediaAsset)
        => Create(vendorId, vendorName, name, description, price, categoryId)
            .Tap(p => p.Id = id)
            .Tap(p => p.IsVisible = isVisible)
            .Tap(p => p.MediaAsset = Maybe<MediaAsset>.From(mediaAsset));

    public static Result<Product> Create(Guid vendorId, string vendorName, string name, string description, decimal price, Guid categoryId)
    {
        var nameResult = name
            .EnsureNotNullOrEmpty(Errors.Create.NameNullOrEmpty)
            .Ensure(n => n.Length >= Constants.NameMinLength, Errors.Create.NameShorterThanMinLength)
            .Ensure(n => n.Length <= Constants.NameMaxLength, Errors.Create.NameLongerThanMaxLength);

        var descriptionResult = description
            .EnsureNotNullOrEmpty(Errors.Create.DescriptionNullOrEmpty)
            .Ensure(d => d.Length >= Constants.DescriptionMinLength,Errors.Create.DescriptionShorterThanMinLength)
            .Ensure(d => d.Length <= Constants.DescriptionMaxLength, Errors.Create.DescriptionLongerThanMaxLength);

        var priceResult = Result.SuccessIf(price > 0, Errors.Create.PriceLessOrEqualToZero);

        var categoryIdResult = Result.SuccessIf(categoryId != Guid.Empty, Errors.Create.CategoryIdEmpty);

        return Result
            .FirstFailureOrSuccess(nameResult, descriptionResult, priceResult, categoryIdResult)
            .Map(() => new Product(vendorId, vendorName, name, description, price, categoryId));
    }

    public Guid CategoryId { get; private set; }

    public Guid VendorId { get; private set; }

    public string VendorName { get; private set; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public FixedPrecisionPrice Price { get; private set; }

    public bool IsVisible { get; private set; }

    public Maybe<MediaAsset> MediaAsset { get; private set; } = Maybe<MediaAsset>.None;

    public Result MakeVisible()
    {
        return Result
            .FailureIf(IsVisible, Errors.MakeVisible.ProductAlreadyVisible)
            .Tap(() => IsVisible = true);
    }

    public Result MakeInvisible()
    {
        return Result
            .SuccessIf(IsVisible, Errors.MakeInvisible.ProductAlreadyInvisible)
            .Tap(() => IsVisible = false);
    }

    public Result ChangeCategory(Guid newCategory)
    {
        return Result
            .SuccessIf(newCategory != Guid.Empty, Errors.ChangeCategory.CategoryIdEmpty)
            .Tap(() => CategoryId = newCategory);
    }

    public Result ChangeDetails(string name, string description, decimal price)
    {
        var nameResult = name
            .EnsureNotNullOrEmpty(Errors.ChangeDetails.NameNullOrEmpty)
            .Ensure(n => n.Length <= Constants.NameMaxLength, Errors.ChangeDetails.NameLongerThanMaxLength)
            .Ensure(n => n.Length >= Constants.NameMinLength, Errors.ChangeDetails.NameShorterThanMinLength);

        var descriptionResult = description
            .EnsureNotNullOrEmpty(Errors.ChangeDetails.DescriptionNullOrEmpty)
            .Ensure(d => d.Length <= Constants.DescriptionMaxLength, Errors.ChangeDetails.DescriptionLongerThanMaxLength)
            .Ensure(d => d.Length >= Constants.DescriptionMinLength, Errors.ChangeDetails.DescriptionShorterThanMinLength);

        var priceResult = Result.SuccessIf(price > 0, Errors.ChangeDetails.PriceLessOrEqualToZero);

        return Result
            .FirstFailureOrSuccess(nameResult, descriptionResult, priceResult)
            .Tap(() =>
            {
                Name = name;
                Description = description;
                Price = price;
            });
    }

    public Result ChangeImage(MediaAsset mediaAsset)
    {
        return Result
            .SuccessIf(mediaAsset is not null, Errors.ChangeImage.MediaAssetNull)
            .Tap(() => MediaAsset = mediaAsset);
    }
}
