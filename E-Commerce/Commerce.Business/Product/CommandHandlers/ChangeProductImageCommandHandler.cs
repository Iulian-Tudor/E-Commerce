using MediatR;
using Commerce.Domain;
using CSharpFunctionalExtensions;

using Errors = Commerce.Business.BusinessErrors.Product.UploadImage;
using static Commerce.Business.BusinessConstants.Product.ImageUpload;
using TimeProvider = Commerce.SharedKernel.Domain.TimeProvider;

namespace Commerce.Business;

internal sealed class ChangeProductImageCommandHandler(IProductRepository productRepository, IBlobStorage blobStorage) : IRequestHandler<ChangeProductImageCommand, Result>
{
    public async Task<Result> Handle(ChangeProductImageCommand request, CancellationToken cancellationToken)
    {
        var productResult = await productRepository.Load(request.ProductId);

        var fileNameSegments = request.FileName.Split('.');
        var extension = fileNameSegments.Last().ToLower();
        var sizeResult = Result.SuccessIf(request.Stream.Length <= MaxFileSize, Errors.SizeExceedsMax);
        var extensionResult = Result.SuccessIf(AllowedExtensions.Contains(extension), Errors.ExtensionNotAccepted);

        return await Result
            .FirstFailureOrSuccess(sizeResult, extensionResult)
            .Bind(() =>
            {
                var imageId = Guid.NewGuid();
                var product = productResult.Value;
                var fileName = $"{product.Name}-{product.Id}-{imageId}.{extension}";

                return BuildBlobFile(request, product, imageId)
                    .Bind(b => blobStorage.Upload(b, fileName));
            })
            .Bind(b => MediaAsset.Create(Guid.NewGuid(), b.RelativePath, b.AbsolutePath, TimeProvider.Instance().UtcNow))
            .Check(productResult.Value.ChangeImage)
            .Tap(() => productRepository.Store(productResult.Value));
    }

    private static Result<BlobFile> BuildBlobFile(ChangeProductImageCommand command, Product productDefinition, Guid imageId)
    {
        var metadata = new Dictionary<string, string>
        {
            { Metadata.ImageId, imageId.ToString() },
            { Metadata.ProductId, productDefinition.Id.ToString() },
            { Metadata.OriginalFileName, command.FileName },
            { Metadata.MimeType, command.MimeType },
            { Metadata.SourceApp, "Commerce" },
            { Metadata.Action, "ChangeProductImage" },
        };

        return BlobFile.Create(command.FileName, command.Stream, command.MimeType, metadata);
    }
}