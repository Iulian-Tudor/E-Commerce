using MediatR;
using CSharpFunctionalExtensions;

namespace Commerce.Business;

public sealed record ConfirmOrderedProductCommand(Guid OrderId, Guid OrderedProductId) : IRequest<Result>;