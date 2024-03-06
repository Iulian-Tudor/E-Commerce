using MediatR;
using CSharpFunctionalExtensions;

using Errors = Commerce.Business.BusinessErrors.Product.Delete;

namespace Commerce.Business;

internal sealed class DeleteProductCommandHandler(IProductRepository productRepository, IIdentifiedUser identifiedUser) : IRequestHandler<DeleteProductCommand, Result>
{
    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var productResult = await productRepository
            .Load(request.ProductId)
            .ToResult(Errors.ProductNotFound);

        var ownershipResult = productResult
            .Ensure(p => p.VendorId == identifiedUser.Id, Errors.ProductDoesNotBelongToCaller);

        return await ownershipResult.Tap(p => productRepository.Delete(p.Id));
    }
}