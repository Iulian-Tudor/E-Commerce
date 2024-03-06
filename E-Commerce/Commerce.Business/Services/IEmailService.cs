using Commerce.Domain;
using CSharpFunctionalExtensions;

namespace Commerce.Business;

public interface IEmailService
{
    public Task<Result> Send(string emailAddress, string subject, string body, string? htmlContent = null);
}

public static class EmailServiceExtensions
{
    public static Task<Result> SendWelcome(this IEmailService service, User user)
    {
        var plainTextContent = $"Hello, {user.FirstName} {user.LastName}! Welcome to E-Commerce!";
        var htmlContent = $"<p>Hello, {user.FirstName} {user.LastName}! Welcome to <strong>E-Commerce</strong>!</p>";

        return service.Send(user.EmailAddress, "Welcome to E-Commerce", plainTextContent, htmlContent);
    }

    public static Task<Result> SendUserGate(this IEmailService service, UserReadModel user, string code)
    {
        var plainTextContent = $"Hello, {user.FirstName} {user.LastName}! Your E-Commerce authentication code is {code}";
        var htmlContent = $"<p>Hello, {user.FirstName} {user.LastName}! Your E-Commerce authentication code is <strong>{code}</strong></p>";

        return service.Send(user.EmailAddress, "E-Commerce Authentication", plainTextContent, htmlContent);
    }
}