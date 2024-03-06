using MediatR;
using CSharpFunctionalExtensions;

namespace Commerce.Business;

public sealed record DeleteProductCommand(Guid ProductId) : IRequest<Result>;

