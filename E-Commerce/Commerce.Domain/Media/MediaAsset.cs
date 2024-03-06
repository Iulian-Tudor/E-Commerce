using CSharpFunctionalExtensions;
using Commerce.SharedKernel.Domain;

using Errors = Commerce.Domain.DomainErrors.MediaAsset;

namespace Commerce.Domain;

public sealed class MediaAsset : ValueObject
{
    private MediaAsset() { }

    private MediaAsset(Guid id, string relativePath, string absolutePath, DateTime timestamp) : this()
    {
        Id = id;
        RelativePath = relativePath;
        AbsolutePath = absolutePath;
        Timestamp = timestamp;
    }

    public static Result<MediaAsset> Create(Guid id, string relativePath, string absolutePath, DateTime timestamp)
    {
        var idResult = id.EnsureNotEmpty(Errors.Create.BadId);
        var relativePathResult = relativePath.EnsureNotNullOrEmpty(Errors.Create.NullOrEmptyRelativePath);
        var absolutePathResult = absolutePath.EnsureNotNullOrEmpty(Errors.Create.NullOrEmptyAbsolutePath);
        var timestampResult = Result.FailureIf(timestamp == DateTime.MinValue, Errors.Create.BadTimestamp);

        return Result.FirstFailureOrSuccess(idResult, relativePathResult, absolutePathResult, timestampResult)
            .Map(() => new MediaAsset(id, relativePath, absolutePath, timestamp));
    }

    public Guid Id { get; private set; }

    public string RelativePath { get; private set; }

    public string AbsolutePath { get; private set; }

    public DateTime Timestamp { get; private set; }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Id;
        yield return RelativePath;
        yield return AbsolutePath;
        yield return Timestamp;
    }
}