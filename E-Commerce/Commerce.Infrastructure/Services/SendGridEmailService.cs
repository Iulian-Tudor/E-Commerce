using SendGrid;
using Commerce.Business;
using SendGrid.Helpers.Mail;
using CSharpFunctionalExtensions;

using Errors = Commerce.Infrastructure.InfrastructureErrors.SendGridEmailService.Send;

namespace Commerce.Infrastructure;

internal sealed class SendGridEmailService(ISendGridClient sendGridClient, SendGridConfiguration configuration) : IEmailService
{
    public async Task<Result> Send(string emailAddress, string subject, string body, string? htmlContent = null)
    {
        var message = new SendGridMessage
        {
            Subject = subject,
            From = new EmailAddress("ecomm.app.noreply@gmail.com", "E-Commerce")
        };

        var to = emailAddress;
        if (configuration.DeveloperEmails.Any() && configuration.DeveloperEmails.All(e => e != emailAddress))
        {
            to = "ecomm.app.noreply@gmail.com";
        }

        message.AddTo(to);
        message.AddContent(MimeType.Html, htmlContent ?? body);

        var sendResult = await sendGridClient.SendEmailAsync(message);

        return Result.SuccessIf(sendResult.IsSuccessStatusCode, Errors.SendFailed);
    }
}