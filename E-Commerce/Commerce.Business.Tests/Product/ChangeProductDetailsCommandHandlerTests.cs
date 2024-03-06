using Xunit;
using NSubstitute;
using Commerce.Domain;
using FluentAssertions;

namespace Commerce.Business.Tests;

public sealed class ChangeProductDetailsCommandHandlerTests
{
	private readonly IProductRepository productRepository = Substitute.For<IProductRepository>();
	private IIdentifiedUser identifiedUser = TestableUser.Any();

    private readonly Product product;

	public ChangeProductDetailsCommandHandlerTests()
    {
        product = ProductsFactory.WithId(identifiedUser.Id);
		productRepository.Load(product.Id).Returns(product);
    }

	[Fact]
	public async Task When_ProductNotFound_Then_ShouldFail()
	{
		// Arrange
		var command = Command() with { ProductId = Guid.NewGuid() };

		// Act
		var result = await Sut().Handle(command, CancellationToken.None);

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Should().Be(BusinessErrors.Product.ChangeDetails.ProductNotFound);

		await productRepository.DidNotReceive().Store(product);
	}

	[Fact]
	public async Task When_ProductDoesNotBelongToCaller_Then_ShouldFail()
	{
		// Arrange
		identifiedUser = TestableUser.Any();

        // Act
        var result = await Sut().Handle(Command(), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
		result.Error.Should().Be(BusinessErrors.Product.ChangeDetails.ProductDoesNotBelongToCaller);

		await productRepository.DidNotReceive().Store(product);
    }

	[Fact]
	public async Task When_DomainFails_Then_ShouldFail()
	{
		// Arrange
		var command = Command() with { Name = "" };
		productRepository.Load(product.Id).Returns(product);

		// Act
		var result = await Sut().Handle(command, CancellationToken.None);

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Should().Be(DomainErrors.Product.ChangeDetails.NameNullOrEmpty);

		await productRepository.DidNotReceive().Store(product);
	}

	[Fact]
	public async Task When_DomainSucceeds_Then_ShouldSucceed()
	{
		// Arrange
		var command = Command();

		// Act
		var result = await Sut().Handle(command, CancellationToken.None);

		// Assert
		result.IsSuccess.Should().BeTrue();

		await productRepository.Received(1).Store(product);
	}

	private ChangeProductDetailsCommand Command() => new(product.Id, "Test Product", "Test Description", 100);

	private ChangeProductDetailsCommandHandler Sut() => new(productRepository, identifiedUser);
}

