using MediatR;
using CSharpFunctionalExtensions;

namespace Commerce.Business;

public sealed record DeleteFavoriteProductCommand(Guid FavoriteProductSnapshotId) : IRequest<Result>;
