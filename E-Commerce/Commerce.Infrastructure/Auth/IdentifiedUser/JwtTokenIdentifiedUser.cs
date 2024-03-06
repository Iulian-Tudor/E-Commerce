using Commerce.Business;
using System.IdentityModel.Tokens.Jwt;

namespace Commerce.Infrastructure;

internal sealed record JwtTokenIdentifiedUser(Guid Id, string EmailAddress, string FirstName, string LastName) : IIdentifiedUser
{
    public static JwtTokenIdentifiedUser From(JwtSecurityToken token)
    {
        var id = Guid.Parse(token.Claims.Single(c => c.Type == CommerceClaims.UserId).Value);
        var emailAddress = token.Claims.Single(c => c.Type == CommerceClaims.UserEmail).Value;
        var firstName = token.Claims.Single(c => c.Type == CommerceClaims.UserFirstName).Value;
        var lastName = token.Claims.Single(c => c.Type == CommerceClaims.UserLastName).Value;

        return new(id, emailAddress, firstName, lastName);
    }
}