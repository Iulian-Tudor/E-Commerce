using Xunit;
using System.Net;
using System.Text;
using Commerce.Domain;
using Newtonsoft.Json;
using FluentAssertions;
using Commerce.Business;

namespace Commerce.Functions.Isolated.Tests.Integration;

public sealed class UserFunctionsCreateUserTests(AzureFunctionsTestContainersFixture fixture) : IClassFixture<AzureFunctionsTestContainersFixture>
{
    [Fact]
    public async Task Given_Create_When_EmailAddressAlreadyInUse_Then_ShouldFail()
    {
        // Arrange
        var requestUri = fixture.Route("api/v1/users");

        var getResponse = await new HttpClient().GetAsync(requestUri);
        var users = await getResponse.DeserializeResponseBody<IEnumerable<UserReadModel>>();

        var command = new CreateUserCommand("John", "Doe", users.First().EmailAddress);
        var reqAsJson = JsonConvert.SerializeObject(command);

        // Act
        var response = await new HttpClient().PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.DeserializeResponseBody<ApiResult>();
        result!.IsSuccess.Should().BeFalse();
        result!.ErrorCode.Should().Be(BusinessErrors.User.Create.EmailAddressAlreadyInUse);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("    ")]
    public async Task Given_Create_When_FirstNameNullOrEmpty_Then_ShouldFail(string badFirstName)
    {
        // Arrange
        var command = new CreateUserCommand(badFirstName, "Doe", "john@mail.ro");
        var requestUri = fixture.Route("api/v1/users");
        var reqAsJson = JsonConvert.SerializeObject(command);

        // Act
        var response = await new HttpClient().PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.DeserializeResponseBody<ApiResult>();
        result!.IsSuccess.Should().BeFalse();
        result!.ErrorCode.Should().Be(DomainErrors.User.Create.FirstNameNullOrEmpty);
    }

    [Fact]
    public async Task Given_Create_When_FirstNameLongerThanMaxLength_Then_ShouldFail()
    {
        // Arrange
        var command = new CreateUserCommand(string.Join("", Enumerable.Repeat("a", DomainConstants.User.FirstNameMaxLength + 1)), "Doe", "john@mail.ru");
        var requestUri = fixture.Route("api/v1/users");
        var reqAsJson = JsonConvert.SerializeObject(command);

        // Act
        var response = await new HttpClient().PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.DeserializeResponseBody<ApiResult>();
        result!.IsSuccess.Should().BeFalse();
        result!.ErrorCode.Should().Be(DomainErrors.User.Create.FirstNameLongerThanMaxLength);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("    ")]
    public async Task Given_Create_When_LastNameNullOrEmpty_Then_ShouldFail(string badLastName)
    {
        // Arrange
        var command = new CreateUserCommand("John", badLastName, "john@mail.ro");
        var requestUri = fixture.Route("api/v1/users");
        var reqAsJson = JsonConvert.SerializeObject(command);

        // Act
        var response = await new HttpClient().PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.DeserializeResponseBody<ApiResult>();
        result!.IsSuccess.Should().BeFalse();
        result!.ErrorCode.Should().Be(DomainErrors.User.Create.LastNameNullOrEmpty);
    }

    [Fact]
    public async Task Given_Create_When_LastNameLongerThanMaxLength_Then_ShouldFail()
    {
        // Arrange
        var command = new CreateUserCommand("John", string.Join("", Enumerable.Repeat("a", DomainConstants.User.LastNameMaxLength + 1)), "john@mail.uk");
        var requestUri = fixture.Route("api/v1/users");
        var reqAsJson = JsonConvert.SerializeObject(command);

        // Act
        var response = await new HttpClient().PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.DeserializeResponseBody<ApiResult>();
        result!.IsSuccess.Should().BeFalse();
        result!.ErrorCode.Should().Be(DomainErrors.User.Create.LastNameLongerThanMaxLength);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("    ")]
    public async Task Given_Create_When_EmailAddressNullOrEmpty_Then_ShouldFail(string badEmailAddress)
    {
        // Arrange
        var command = new CreateUserCommand("John", "Doe", badEmailAddress);
        var requestUri = fixture.Route("api/v1/users");
        var reqAsJson = JsonConvert.SerializeObject(command);

        // Act
        var response = await new HttpClient().PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.DeserializeResponseBody<ApiResult>();
        result!.IsSuccess.Should().BeFalse();
        result!.ErrorCode.Should().Be(DomainErrors.User.Create.EmailAddressNullOrEmpty);
    }

    [Fact]
    public async Task Given_Create_When_EmailAddressLongerThanMaxLength_Then_ShouldFail()
    {
        // Arrange
        var command = new CreateUserCommand("John", "Doe", string.Join("", Enumerable.Repeat("a", DomainConstants.User.EmailAddressMaxLength + 1)));
        var requestUri = fixture.Route("api/v1/users");
        var reqAsJson = JsonConvert.SerializeObject(command);

        // Act
        var response = await new HttpClient().PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.DeserializeResponseBody<ApiResult>();
        result!.IsSuccess.Should().BeFalse();
        result!.ErrorCode.Should().Be(DomainErrors.User.Create.EmailAddressLongerThanMaxLength);
    }

    [Theory]
    [InlineData("john")]
    [InlineData("john@mail")]
    [InlineData("john@mail.")]
    [InlineData("john@mail@com")]
    public async Task Given_Create_When_EmailAddressInvalid_Then_ShouldFail(string badEmailAddress)
    {
        // Arrange
        var command = new CreateUserCommand("John", "Doe", badEmailAddress);
        var requestUri = fixture.Route("api/v1/users");
        var reqAsJson = JsonConvert.SerializeObject(command);

        // Act
        var response = await new HttpClient().PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.DeserializeResponseBody<ApiResult>();
        result!.IsSuccess.Should().BeFalse();
        result!.ErrorCode.Should().Be(DomainErrors.User.Create.EmailAddressInvalidFormat);
    }

    [Fact]
    public async Task Given_Create_When_NotViolatingConstraints_Then_ShouldSucceed()
    {
        // Arrange
        var command = new CreateUserCommand("John", "Doe", "john.doe@gmail.co.uk");
        var requestUri = fixture.Route("api/v1/users");
        var reqAsJson = JsonConvert.SerializeObject(command);

        // Act
        var response = await new HttpClient().PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var result = await response.DeserializeResponseBody<ApiResult>();
        result.Should().BeNull();
    }
}