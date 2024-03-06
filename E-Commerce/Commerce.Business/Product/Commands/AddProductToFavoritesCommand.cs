using MediatR;
using CSharpFunctionalExtensions;

namespace Commerce.Business;

public sealed record AddProductToFavoritesCommand(Guid ProductId) : IRequest<Result>;