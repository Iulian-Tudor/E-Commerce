using Commerce.Business;
using Azure.Storage.Blobs;
using CSharpFunctionalExtensions;
using Commerce.SharedKernel.Domain;

using Errors = Commerce.Infrastructure.InfrastructureErrors.AzureBlobStorage;

namespace Commerce.Infrastructure;

public sealed class AzureBlobStorage(BlobServiceClient blobServiceClient) : IBlobStorage
{
    public Uri GetContainerUri() => blobServiceClient.Uri;

    public async Task<Result<UploadedFile>> Upload(BlobFile blob, string fileName)
    {
        var container = "commerce";
        if (!await blobServiceClient.GetBlobContainerClient(container).ExistsAsync())
        {
            await blobServiceClient.CreateBlobContainerAsync(container);
        }
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(container);
        fileName = fileName.Replace(" ", string.Empty);

        var blobResult = blob.EnsureNotNull(Errors.Upload.BlobNull);
        var fileNameResult = fileName.EnsureNotNullOrEmpty(Errors.Upload.BlobNameNullOrEmpty);

        return await Result
            .FirstFailureOrSuccess(blobResult, fileNameResult)
            .Map(() => blobContainerClient.GetBlobClient(fileName))
            .Tap(b => b.UploadAsync(blob.Content))
            .Map(b => new UploadedFile(fileName, b.Uri.MakeRelativeUri(blobContainerClient.Uri).ToString(), b.Uri.AbsolutePath));
    }

    public async Task<Result> Delete(string fileName)
    {
        return Result.SuccessIf(await blobServiceClient.GetBlobContainerClient("commerce").DeleteBlobIfExistsAsync(fileName), "Failed to delete blob. Blob does not exist or something else went wrong");
    }

    public async Task<Result<DownloadedFile>> Get(string fileName)
    {
        var container = "commerce";
        if (!await blobServiceClient.GetBlobContainerClient(container).ExistsAsync())
        {
            await blobServiceClient.CreateBlobContainerAsync(container);
        }
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(container);

        var blobResult = fileName.EnsureNotNullOrEmpty("Invalid blob name.");

        var name = fileName.Split('/').Last();

        return await Result
            .FirstFailureOrSuccess(blobResult)
            .Map(() => blobContainerClient.GetBlobClient(name))
            .Map(async b =>
            {
                var memoryStream = new MemoryStream();

                try
                {
                    await b.DownloadToAsync(memoryStream);
                }
                catch (Exception ex)
                {
                    return Array.Empty<byte>();
                }

                return memoryStream.ToArray();
            })
            .Map(r => new DownloadedFile { Data = r });
    }
}