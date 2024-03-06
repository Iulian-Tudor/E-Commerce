using FluentAssertions;
using Xunit;

namespace Commerce.Domain.Tests;

public sealed class UserChangeDetailsTests
{
    private readonly User sut = UsersFactory.Any();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Given_ChangeDetails_When_FirstNameIsNullOrEmpty_Then_ShouldFail(string firstName)
    {
        // Act
        var result = sut.ChangeDetails(firstName, "lastName");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.ChangeDetails.FirstNameNullOrEmpty);
    }

    [Fact]
    public void Given_ChangeDetails_When_FirstNameIsLongerThanMax_Then_ShouldFail()
    {
        // Act
        var result = sut.ChangeDetails(new string('a', DomainConstants.User.FirstNameMaxLength + 1), "lastName");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.ChangeDetails.FirstNameLongerThanMaxLength);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Given_ChangeDetails_When_LastNameIsNullOrEmpty_Then_ShouldFail(string lastName)
    {
        // Act
        var result = sut.ChangeDetails("firstName", lastName);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.ChangeDetails.LastNameNullOrEmpty);
    }

    [Fact]
    public void Given_ChangeDetails_When_LastNameIsLongerThanMax_Then_ShouldFail()
    {
        // Act
        var result = sut.ChangeDetails("firstName", new string('a', DomainConstants.User.LastNameMaxLength + 1));

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.ChangeDetails.LastNameLongerThanMaxLength);
    }

    [Fact]
    public void Given_Create_When_NotViolatingConstraints_Then_ShouldSucceed()
    {
        // Arrange
        var firstName = "firstName";
        var lastName = "lastName";

        // Act
        var result = sut.ChangeDetails(firstName, lastName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.FirstName.Should().Be(firstName);
        sut.LastName.Should().Be(lastName);
    }
}