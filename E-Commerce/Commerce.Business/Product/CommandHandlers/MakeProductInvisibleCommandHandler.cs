using MediatR;
using CSharpFunctionalExtensions;

using Errors = Commerce.Business.BusinessErrors.Product.MakeInvisible;

namespace Commerce.Business;

internal sealed class MakeProductInvisibleCommandHandler(IProductRepository productRepository, IIdentifiedUser identifiedUser) : IRequestHandler<MakeProductInvisibleCommand, Result>
{
    public async Task<Result> Handle(MakeProductInvisibleCommand request, CancellationToken cancellationToken)
    {
        var productResult = await productRepository
            .Load(request.ProductId)
            .ToResult(Errors.ProductNotFound);

        var ownershipResult = productResult
            .Ensure(p => p.VendorId == identifiedUser.Id, Errors.ProductDoesNotBelongToCaller);

        return await ownershipResult
            .Bind(p => p.MakeInvisible())
            .Tap(() => productRepository.Store(productResult.Value));
    }
}
