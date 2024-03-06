using Xunit;
using NSubstitute;
using FluentAssertions;
using CSharpFunctionalExtensions;

using TimeProvider = Commerce.SharedKernel.Domain.TimeProvider;

namespace Commerce.Business.Tests;

public sealed class RefreshTokenCommandHandlerTests
{
    private readonly ITokenService tokenService = Substitute.For<ITokenService>();

    [Fact]
    public async Task When_TokenServiceFails_Then_ShouldFail()
    {
        // Arrange
        tokenService
            .Refresh(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Result.Failure<(string, Token)>("fail"));

        // Act
        var result = await Sut().Handle(Command(), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("fail");

        await tokenService.Received(1).Refresh(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task When_TokenServiceSucceeds_Then_ShouldSucceed()
    {
        // Arrange
        tokenService
            .Refresh(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Result.Success(("token", new Token("token", TimeProvider.Instance().UtcNow))));

        // Act
        var result = await Sut().Handle(Command(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.AuthToken.Should().NotBeNullOrWhiteSpace();
        result.Value.RefreshToken.Should().NotBeNull();

        await tokenService.Received(1).Refresh(Arg.Any<string>(), Arg.Any<string>());
    }

    private RefreshTokenCommand Command() => new("token", "refreshToken");

    private RefreshTokenCommandHandler Sut() => new(tokenService);
}