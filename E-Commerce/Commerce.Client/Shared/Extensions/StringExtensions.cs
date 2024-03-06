namespace Commerce.Client.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string? subject) => string.IsNullOrEmpty(subject?.Trim());
}