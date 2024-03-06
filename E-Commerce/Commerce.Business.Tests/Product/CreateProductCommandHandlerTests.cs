using Xunit;
using NSubstitute;
using Commerce.Domain;
using CSharpFunctionalExtensions;
using FluentAssertions;

namespace Commerce.Business.Tests;
public sealed class CreateProductCommandHandlerTests
{
	private readonly IProductRepository productRepository = Substitute.For<IProductRepository>();
	private readonly IUserRepository userRepository = Substitute.For<IUserRepository>();

    private readonly User user = UsersFactory.Any();

	public CreateProductCommandHandlerTests()
	{
		userRepository.Load(user.Id).Returns(user);
	}

	[Fact]
	public async Task When_UserNotFound_Then_ShouldFail()
	{
		// Arrange
		var command = Command();
		userRepository.Load(command.VendorId).Returns(Maybe<User>.None);

		// Act
		var result = await Sut().Handle(command, CancellationToken.None);

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Should().Be(BusinessErrors.Product.Create.UserNotFound);

		await productRepository.DidNotReceive().Store(Arg.Any<Product>());
	}

	[Fact]
	public async Task When_DomainFails_Then_ShouldFail()
	{
		// Arrange
		var command = Command() with { Name = string.Empty};

		// Act
		var result = await Sut().Handle(command, CancellationToken.None);

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Should().Be(DomainErrors.Product.Create.NameNullOrEmpty);

		await productRepository.DidNotReceive().Store(Arg.Any<Product>());
	}


	[Fact]
	public async Task When_DomainSucceeds_Then_ShouldSucceed()
	{
		// Act
		var result = await Sut().Handle(Command(), CancellationToken.None);

		// Assert
		result.IsSuccess.Should().BeTrue();

		await productRepository.Received(1).Store(Arg.Any<Product>());
	}

	private CreateProductCommand Command() => new(user.Id, "Test Product", "Test Description", 100, Guid.NewGuid());

	private CreateProductCommandHandler Sut() => new(productRepository, userRepository);
}
