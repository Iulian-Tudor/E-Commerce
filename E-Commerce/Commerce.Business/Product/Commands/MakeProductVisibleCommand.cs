using MediatR;
using CSharpFunctionalExtensions;

namespace Commerce.Business;

public sealed record MakeProductVisibleCommand(Guid ProductId) : IRequest<Result>;
