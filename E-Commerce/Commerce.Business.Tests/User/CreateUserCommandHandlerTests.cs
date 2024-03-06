using Xunit;
using NSubstitute;
using Commerce.Domain;
using FluentAssertions;

namespace Commerce.Business.Tests;

public sealed class CreateUserCommandHandlerTests
{
    private readonly IUserRepository userRepository = Substitute.For<IUserRepository>();
    private readonly IEmailService emailService = Substitute.For<IEmailService>();

    public CreateUserCommandHandlerTests()
    {
        userRepository.Query().Returns(new List<UserReadModel>().AsQueryable());
    }

    [Fact]
    public async Task When_EmailAddressAlreadyUsed_Then_ShouldFail()
    {
        // Arrange
        var existingUser = new UserReadModel().FromAggregate(UsersFactory.Any());
        userRepository.Query().Returns(new List<UserReadModel> { existingUser }.AsQueryable());

        var command = Command() with { EmailAddress = existingUser.EmailAddress };

        // Act
        var result = await Sut().Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BusinessErrors.User.Create.EmailAddressAlreadyInUse);

        await userRepository.DidNotReceive().Store(Arg.Any<User>());
    }

    [Fact]
    public async Task When_DomainFails_Then_ShouldFail()
    {
        // Arrange
        var command = Command() with { FirstName = string.Empty };

        // Act
        var result = await Sut().Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.Create.FirstNameNullOrEmpty);

        await userRepository.DidNotReceive().Store(Arg.Any<User>());
    }

    [Fact]
    public async Task When_DomainSucceeds_Then_ShouldSucceed()
    {
        // Act
        var result = await Sut().Handle(Command(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        await userRepository.Received(1).Store(Arg.Any<User>());
    }
    
    private CreateUserCommand Command() => new("First Name", "Last Name", "mail@mail.com");

    private CreateUserCommandHandler Sut() => new(userRepository, emailService);
}