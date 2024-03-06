using MediatR;
using CSharpFunctionalExtensions;

namespace Commerce.Business;

public sealed record ChangeProductCategoryCommand(Guid ProductId, Guid CategoryId, Guid VendorId, string Name, string Description, decimal Price): IRequest<Result>;