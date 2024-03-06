using Xunit;
using NSubstitute;
using Commerce.Domain;
using FluentAssertions;

namespace Commerce.Business.Tests;

public sealed class MakeProductVisibleCommandHandlerTests
{
	private readonly IProductRepository productRepository = Substitute.For<IProductRepository>();
	private IIdentifiedUser identifiedUser = TestableUser.Any();

	private readonly Product product;

	public MakeProductVisibleCommandHandlerTests()
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
		result.Error.Should().Be(BusinessErrors.Product.MakeVisible.ProductNotFound);

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
		result.Error.Should().Be(BusinessErrors.Product.MakeVisible.ProductDoesNotBelongToCaller);

		await productRepository.DidNotReceive().Store(product);
	}

	[Fact]
	public async Task When_DomainFails_Then_ShouldFail()
	{
		// Arrange
		product.MakeVisible();

		// Act
		var result = await Sut().Handle(Command(), CancellationToken.None);

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Should().Be(DomainErrors.Product.MakeVisible.ProductAlreadyVisible);

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

	private MakeProductVisibleCommand Command() => new(product.Id);
	private MakeProductVisibleCommandHandler Sut() => new(productRepository, identifiedUser);
}