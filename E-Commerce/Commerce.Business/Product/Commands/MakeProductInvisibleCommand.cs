using MediatR;
using CSharpFunctionalExtensions;

namespace Commerce.Business;

public sealed record MakeProductInvisibleCommand(Guid ProductId) : IRequest<Result>;