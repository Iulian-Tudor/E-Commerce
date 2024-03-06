using CSharpFunctionalExtensions;

namespace Commerce.Business;
public interface IIdentifiedUserAccessor
{
    Maybe<IIdentifiedUser> User { get; }

    void Set(Maybe<IIdentifiedUser> user);
}