using Xunit;
using NSubstitute;
using Commerce.Domain;
using FluentAssertions;

namespace Commerce.Business.Tests;

public sealed class ChangeUserDetailsCommandHandlerTests
{
    private readonly IUserRepository userRepository = Substitute.For<IUserRepository>();
    private readonly User user = UsersFactory.Any();

    public ChangeUserDetailsCommandHandlerTests()
    {
        userRepository.Load(user.Id).Returns(user);
    }

    [Fact]
    public async Task When_UserNotFound_Then_ShouldFail()
    {
        // Arrange
        var command = Command() with { UserId = Guid.NewGuid() };

        // Act
        var result = await Sut().Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BusinessErrors.User.ChangeDetails.UserNotFound);
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
        result.Error.Should().Be(DomainErrors.User.ChangeDetails.FirstNameNullOrEmpty);
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

    private ChangeUserDetailsCommand Command() => new(user.Id, "new name", "new last name");

    private ChangeUserDetailsCommandHandler Sut() => new(userRepository);
}