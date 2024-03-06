using Xunit;
using NSubstitute;
using Commerce.Domain;
using FluentAssertions;
using CSharpFunctionalExtensions;

namespace Commerce.Business.Tests;

public sealed class DeleteProductCommandHandlerTests
{
	private readonly IProductRepository productRepository = Substitute.For<IProductRepository>();
	private readonly IIdentifiedUser identifiedUser = Substitute.For<IIdentifiedUser>();

	public DeleteProductCommandHandlerTests()
	{
		identifiedUser.Id.Returns(Guid.NewGuid());
	}

	[Fact]
	public async Task When_ProductNotFound_Then_ShouldFail()
	{
		// Arrange
		var command = Command();
		productRepository.Load(command.ProductId).Returns(Maybe<Product>.None);

		// Act
		var result = await Sut().Handle(command, CancellationToken.None);

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Should().Be(BusinessErrors.Product.Delete.ProductNotFound);

		await productRepository.DidNotReceive().Delete(Arg.Any<Guid>());
	}

	[Fact]
	public async Task When_ProductDoesNotBelongToCaller_Then_ShouldFail()
	{
		// Arrange
		var command = Command();
		var product = Product.Create(Guid.NewGuid(), Guid.NewGuid(), "Test Vendor", "Test Product", "Test Description", 100, Guid.NewGuid(), true, null).Value;
		productRepository.Load(command.ProductId).Returns(product);

		// Act
		var result = await Sut().Handle(command, CancellationToken.None);

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Should().Be(BusinessErrors.Product.Delete.ProductDoesNotBelongToCaller);

		await productRepository.DidNotReceive().Delete(Arg.Any<Guid>());
	}

	[Fact]
	public async Task When_ProductIsDeleted_Then_ShouldSucceed()
	{
		// Arrange
		var command = Command();
		var product = Product.Create(Guid.NewGuid(), identifiedUser.Id, "Test Vendor", "Test Product", "Test Description", 100, Guid.NewGuid(), true, null).Value;
		productRepository.Load(command.ProductId).Returns(product);

		// Act
		var result = await Sut().Handle(command, CancellationToken.None);

		// Assert
		result.IsSuccess.Should().BeTrue();

		await productRepository.Received(1).Delete(Arg.Any<Guid>());
	}

	[Fact]
	public async Task When_NotViolatingConstraints_Then_ShouldSucceed()
	{
		// Arrange
		var command = Command();
		var product = Product.Create(Guid.NewGuid(), identifiedUser.Id, "Test Vendor", "Test Product", "Test Description", 100, Guid.NewGuid(), true, null).Value;
		productRepository.Load(command.ProductId).Returns(product);

		// Act
		var result = await Sut().Handle(command, CancellationToken.None);

		// Assert
		result.IsSuccess.Should().BeTrue();

		await productRepository.Received(1).Delete(Arg.Any<Guid>());
	}

	private DeleteProductCommand Command() => new(Guid.NewGuid());

	private DeleteProductCommandHandler Sut() => new(productRepository, identifiedUser);
}
