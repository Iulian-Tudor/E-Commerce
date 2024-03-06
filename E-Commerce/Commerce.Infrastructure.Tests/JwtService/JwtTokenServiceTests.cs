using Xunit;
using NSubstitute;
using Commerce.Domain;
using FluentAssertions;
using Commerce.Business;

namespace Commerce.Infrastructure.Tests;

public sealed class JwtTokenServiceTests
{
    private readonly User user = UsersFactory.Any();

    private readonly JwtConfiguration jwtConfiguration = new()
    {
        Issuer = "issuer",
        TokenLifetimeMinutes = 15,
        Audience = "audience",
        Key = "superdupermegaultrasecretkeyhopefully"
    };

    [Fact]
    public void Given_GenerateJwtToken_When_UserNotProvided_Then_ShouldFail()
    {
        //Act
        var result = Sut().GenerateAuthToken(null);

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InfrastructureErrors.JwtTokenService.GenerateToken.UserNotProvided);
    }

    [Fact]
    public void Given_GenerateJwtToken_When_UserBelongsToTenant_Then_ShouldSucceed()
    {
        //Act
        var result = Sut().GenerateAuthToken(user);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    private JwtTokenService Sut() => new(jwtConfiguration, Substitute.For<IJwtTokenRepository>());
}