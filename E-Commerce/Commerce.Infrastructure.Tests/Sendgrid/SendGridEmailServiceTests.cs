using Xunit;
using SendGrid;
using System.Net;
using NSubstitute;
using FluentAssertions;
using SendGrid.Helpers.Mail;

namespace Commerce.Infrastructure.Tests;

public sealed class SendGridEmailServiceTests
{
    private readonly ISendGridClient sendGridClient = Substitute.For<ISendGridClient>();
    private readonly SendGridConfiguration configuration = new()
    {
        DeveloperEmails = new List<string> { "tudor.tescu@hotmail.com" }
    };

    [Fact]
    public async Task Given_Send_When_SendGridClientFails_Then_ShouldFail()
    {
        // Arrange
        sendGridClient
            .SendEmailAsync(Arg.Any<SendGridMessage>())
            .Returns(new Response(HttpStatusCode.NotFound, new StringContent(""), null));

        // Act
        var result = await Sut().Send("tudor.tescu@hotmail.com", "Test", "Test");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InfrastructureErrors.SendGridEmailService.Send.SendFailed);
    }

    [Fact]
	public async Task Given_Send_When_SendGridClientSucceeds_Then_ShouldSucceed()
    {
        // Arrange
        sendGridClient
            .SendEmailAsync(Arg.Any<SendGridMessage>())
            .Returns(new Response(HttpStatusCode.OK, new StringContent(""), null));

        // Act
        var result = await Sut().Send("tudor.tescu@hotmail.com", "Test", "Test");

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    private SendGridEmailService Sut() => new(sendGridClient, configuration);
}