using MediatR;
using Commerce.Domain;
using CSharpFunctionalExtensions;

using Errors = Commerce.Business.BusinessErrors.Product.Create;

namespace Commerce.Business;

internal sealed class CreateProductCommandHandler(IProductRepository productRepository, IUserRepository userRepository) : IRequestHandler<CreateProductCommand, Result>
{
    public async Task<Result> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        return await userRepository
            .Load(request.VendorId)
            .ToResult(Errors.UserNotFound)
            .Bind(u => Product.Create(request.VendorId, $"{u.FirstName} {u.LastName}", request.Name, request.Description, request.Price, request.CategoryId))
            .Tap(productRepository.Store);
    }
}