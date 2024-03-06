using CSharpFunctionalExtensions;

namespace Commerce.SharedKernel.Domain;

public static class StringExtensions
{
    public static Result<string> EnsureNotNullOrEmpty(this string subject, string error) => Result.FailureIf(string.IsNullOrEmpty(subject?.Trim()), subject!, error);

    public static Result<T> EnsureNotNull<T>(this T subject, string error) => Result.FailureIf(subject == null, subject, error);
}