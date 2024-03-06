using TimeProvider = Commerce.SharedKernel.Domain.TimeProvider;

namespace Commerce.Domain;

public sealed class MediaAssetsFactory
{
    public static MediaAsset Any() => MediaAsset.Create(Guid.NewGuid(), "relative", "/absolute", TimeProvider.Instance().UtcNow).Value;
}