using Xunit;
using FluentAssertions;

using TimeProvider = Commerce.SharedKernel.Domain.TimeProvider;

namespace Commerce.Domain.Tests;

public sealed class MediaAssetCreateTests
{
    private readonly Guid id = Guid.NewGuid();
    private readonly string relativePath = "relative";
    private readonly string absolutePath = "/absolute";
    private readonly DateTime timestamp = TimeProvider.Instance().UtcNow;

    [Fact]
    public void Given_Create_When_IdIsEmpty_Then_ShouldFail()
    {
        // Act
        var result = MediaAsset.Create(Guid.Empty, relativePath, absolutePath, timestamp);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.MediaAsset.Create.BadId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void Given_Create_When_RelativePathIsNullOrEmpty_Then_ShouldFail(string relativePath)
    {
        // Act
        var result = MediaAsset.Create(id, relativePath, absolutePath, timestamp);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.MediaAsset.Create.NullOrEmptyRelativePath);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void Given_Create_When_AbsolutePathIsNullOrEmpty_Then_ShouldFail(string absolutePath)
    {
        // Act
        var result = MediaAsset.Create(id, relativePath, absolutePath, timestamp);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.MediaAsset.Create.NullOrEmptyAbsolutePath);
    }

    [Fact]
    public void Given_Create_When_TimestampIsMinValue_Then_ShouldFail()
    {
        // Act
        var result = MediaAsset.Create(id, relativePath, absolutePath, DateTime.MinValue);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.MediaAsset.Create.BadTimestamp);
    }

    [Fact]
    public void Given_Create_When_NotViolatingConstraints_Then_ShouldCreate()
    {
        // Act
        var result = MediaAsset.Create(id, relativePath, absolutePath, timestamp);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.Id.Should().Be(id);
        result.Value.RelativePath.Should().Be(relativePath);
        result.Value.AbsolutePath.Should().Be(absolutePath);
        result.Value.Timestamp.Should().Be(timestamp);
    }
}