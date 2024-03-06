using CSharpFunctionalExtensions;

namespace Commerce.SharedKernel.Domain;

public static class GuidExtensions
{
    public static Result<Guid> EnsureNotEmpty(this Guid subject, string error)
    {
        return Result.FailureIf(subject == Guid.Empty, subject, error);
    }
}