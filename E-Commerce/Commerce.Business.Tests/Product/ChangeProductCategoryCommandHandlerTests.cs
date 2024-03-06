using Xunit;
using NSubstitute;
using Commerce.Domain;
using CSharpFunctionalExtensions;
using FluentAssertions;

namespace Commerce.Business.Tests;

public sealed class ChangeProductCategoryCommandHandlerTests
{
    private readonly IProductRepository productRepository = Substitute.For<IProductRepository>();
    private readonly ICategoryRepository categoryRepository = Substitute.For<ICategoryRepository>();
    private IIdentifiedUser identifiedUser = TestableUser.Any();
    private readonly Product product = ProductsFactory.Any();

    public ChangeProductCategoryCommandHandlerTests()
    {
        productRepository.Load(product.Id).Returns(product);
    }

    [Fact]
    public async Task When_ProductNotFound_Then_ShouldFail()
    {
        // Arrange
        var command = Command() with { ProductId = Guid.NewGuid() };
        var category = CategoriesFactory.WithId(product.CategoryId);

        categoryRepository.Load(command.CategoryId).Returns(category);
        productRepository.Load(command.ProductId).Returns(Maybe<Product>.None);

        // Act
        var result = await Sut().Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BusinessErrors.Product.ChangeCategory.ProductNotFound);

        await productRepository.DidNotReceive().Store(product);
    }

    [Fact]
    public async Task When_CategoryNotFound_Then_ShouldFail()
    {
        // Arrange
        var command = Command() with { CategoryId = Guid.NewGuid() };
        categoryRepository.Load(command.CategoryId).Returns(Maybe<Category>.None);

        // Act
        var result = await Sut().Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BusinessErrors.Product.ChangeCategory.CategoryNotFound);

        await productRepository.DidNotReceive().Store(product);
    }

    [Fact]
    public async Task When_ProductDoesNotBelongToCaller_Then_ShouldFail()
    {
        // Arrange
        var command = Command();
        var category = CategoriesFactory.WithId(product.CategoryId);

        categoryRepository.Load(command.CategoryId).Returns(category);
        productRepository.Load(command.ProductId).Returns(product);

        // Act
        var result = await Sut().Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BusinessErrors.Product.ChangeCategory.ProductDoesNotBelongToCaller);

        await productRepository.DidNotReceive().Store(product);
    }

    [Fact]
    public async Task When_DomainFails_Then_ShouldFail()
    {
        //Arrange
        var command = Command() with { CategoryId = Guid.Empty };
        var category = CategoriesFactory.Any();

        categoryRepository.Load(command.CategoryId).Returns(category);
        productRepository.Load(command.ProductId).Returns(product);

        identifiedUser = TestableUser.WithId(product.VendorId);

        // Act
        var result = await Sut().Handle(command, CancellationToken.None);
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Product.ChangeCategory.CategoryIdEmpty);
        await productRepository.DidNotReceive().Store(product);
    }

    [Fact]
    public async Task When_DomainSucceeds_Then_ShouldSucceed()
    {
        // Arrange
        var command = Command();
        var category = CategoriesFactory.Any();

        categoryRepository.Load(command.CategoryId).Returns(category);
        productRepository.Load(command.ProductId).Returns(product);

        identifiedUser = TestableUser.WithId(product.VendorId);

        // Act
        var result = await Sut().Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await productRepository.Received(1).Store(product);
    }
    
    private ChangeProductCategoryCommand Command() => new(product.Id, product.CategoryId,product.VendorId, product.Name,product.Description,product.Price);

    private ChangeProductCategoryCommandHandler Sut() => new(productRepository, categoryRepository, identifiedUser);
}