using System.IdentityModel.Tokens.Jwt;

namespace Commerce.Client;

public sealed record IdentifiedUser(Guid Id, string EmailAddress, string FirstName, string LastName)
{
    public static IdentifiedUser From(JwtSecurityToken token)
    {
        var id = Guid.Parse(token.Claims.Single(c => c.Type == CommerceClaims.UserId).Value);
        var emailAddress = token.Claims.Single(c => c.Type == CommerceClaims.UserEmail).Value;
        var firstName = token.Claims.Single(c => c.Type == CommerceClaims.UserFirstName).Value;
        var lastName = token.Claims.Single(c => c.Type == CommerceClaims.UserLastName).Value;

        return new(id, emailAddress, firstName, lastName);
    }

    public string Initials => $"{FirstName[0]}{LastName[0]}";

    public string FullName => $"{FirstName} {LastName}";
}