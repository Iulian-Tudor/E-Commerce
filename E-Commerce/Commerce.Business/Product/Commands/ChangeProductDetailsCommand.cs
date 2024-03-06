using MediatR;
using CSharpFunctionalExtensions;

namespace Commerce.Business;

public sealed record ChangeProductDetailsCommand(Guid ProductId, string Name, string Description, decimal Price) : IRequest<Result>;