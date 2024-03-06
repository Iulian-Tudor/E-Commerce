using CSharpFunctionalExtensions;
using Commerce.SharedKernel.Domain;

namespace Commerce.Business;

public interface IBlobStorage
{
    Uri GetContainerUri();

    Task<Result<UploadedFile>> Upload(BlobFile blob, string fileName);

    Task<Result> Delete(string fileName);

    Task<Result<DownloadedFile>> Get(string fileName);
}

public class DownloadedFile
{
    public byte[] Data { get; set; } = Array.Empty<byte>();
}

public record UploadedFile(string FileName, string RelativePath, string AbsolutePath);

public sealed class BlobFile
{
    private BlobFile(string originalName, Stream content, string mimeType, IDictionary<string, string> metadata)
    {
        OriginalName = originalName;
        Content = content;
        MimeType = mimeType;
        Metadata = metadata;
    }

    public static Result<BlobFile> Create(string originalName, Stream content, string mimeType, IDictionary<string, string> metadata)
    {
        var nameResult = originalName.EnsureNotNullOrEmpty("Invalid file name!");
        var contentResult = Result.SuccessIf(content.Length > 0, "Invalid file content!");
        var mimeTypeResult = mimeType.EnsureNotNullOrEmpty("Invalid mime type!");
        var metadataResult = metadata.EnsureNotNull("Metadata is not optional!");

        return Result.FirstFailureOrSuccess(nameResult, contentResult, mimeTypeResult, metadataResult)
            .Map(() => new BlobFile(originalName, content, mimeType, metadata));
    }

    public string OriginalName { get; private set; }

    public Stream Content { get; private set; }

    public string MimeType { get; private set; }

    public IDictionary<string, string> Metadata { get; private set; }
}