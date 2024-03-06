using MediatR;

namespace Commerce.Business;

internal sealed class GetUsersQueryHandler(IUserRepository repository) : IRequestHandler<GetUsersQuery, IReadOnlyCollection<UserReadModel>>
{
    public async Task<IReadOnlyCollection<UserReadModel>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = repository.Query();

        return request.Id == null
            ? users.ToList()
            : users.Where(u => u.Id == request.Id).ToList();
    }
}