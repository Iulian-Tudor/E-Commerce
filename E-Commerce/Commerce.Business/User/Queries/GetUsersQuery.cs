using MediatR;

namespace Commerce.Business;

public sealed record GetUsersQuery : IRequest<IReadOnlyCollection<UserReadModel>>
{
    public Guid? Id { get; init; }
}