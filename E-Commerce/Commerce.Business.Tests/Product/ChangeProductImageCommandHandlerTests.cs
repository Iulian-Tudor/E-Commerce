using Xunit;
using NSubstitute;
using Commerce.Domain;
using FluentAssertions;
using CSharpFunctionalExtensions;

namespace Commerce.Business.Tests;

public sealed class ChangeProductImageCommandHandlerTests
{
    private readonly IProductRepository productRepository = Substitute.For<IProductRepository>();
    private readonly IBlobStorage blobStorage = Substitute.For<IBlobStorage>();

    private readonly Product product = ProductsFactory.Any();

    public ChangeProductImageCommandHandlerTests()
    {
        productRepository.Load(product.Id).Returns(product);
        blobStorage.Upload(Arg.Any<BlobFile>(), Arg.Any<string>()).Returns(Result.Success(new UploadedFile("name", "relative", "absolute")));
    }

    [Fact]
    public async Task When_FileSizeExceedsMax_Then_ShouldFail()
    {
        // Arrange
        var command = Command() with { Stream = new MemoryStream(new byte[BusinessConstants.Product.ImageUpload.MaxFileSize * 1024 + 1]) };

        // Act
        var result = await Sut().Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BusinessErrors.Product.UploadImage.SizeExceedsMax);

        await productRepository.DidNotReceive().Store(product);
    }

    [Theory]
    [InlineData("gif")]
    [InlineData("exe")]
    [InlineData("zip")]
    [InlineData("cs")]
    [InlineData("csproj")]
    [InlineData("sln")]
    [InlineData("cshtml")]
    public async Task When_FileExtensionIsInvalid_Then_ShouldFail(string extension)
    {
        // Arrange
        var command = Command() with { FileName = $"filename.{extension}" };

        // Act
        var result = await Sut().Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BusinessErrors.Product.UploadImage.ExtensionNotAccepted);

        await productRepository.DidNotReceive().Store(product);
    }

    [Fact]
    public async Task When_DomainSucceeds_Then_ShouldSucceed()
    {
        // Act
        var result = await Sut().Handle(Command(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await productRepository.Received(1).Store(product);
    }

    private ChangeProductImageCommand Command() => new(product.Id, new MemoryStream([1, 2, 3]), "filename.jpg", "image/jpeg");

    private ChangeProductImageCommandHandler Sut() => new(productRepository, blobStorage);
}