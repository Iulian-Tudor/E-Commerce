using Commerce.Business;
using CSharpFunctionalExtensions;

namespace Commerce.Infrastructure;

internal sealed class IdentifiedUserAccessor : IIdentifiedUserAccessor
{
    public Maybe<IIdentifiedUser> User { get; private set; } = Maybe<IIdentifiedUser>.None;

    public void Set(Maybe<IIdentifiedUser> user) => User = user;
}