using MediatR;
using CSharpFunctionalExtensions;

namespace Commerce.Business;

public sealed record CreateProductCommand(Guid VendorId, string Name, string Description, decimal Price, Guid CategoryId) : IRequest<Result>;