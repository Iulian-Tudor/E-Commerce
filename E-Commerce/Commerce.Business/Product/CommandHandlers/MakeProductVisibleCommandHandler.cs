using MediatR;
using CSharpFunctionalExtensions;

using Errors = Commerce.Business.BusinessErrors.Product.MakeVisible;

namespace Commerce.Business;

internal sealed class MakeProductVisibleCommandHandler(IProductRepository productRepository, IIdentifiedUser identifiedUser) : IRequestHandler<MakeProductVisibleCommand, Result>
{
    public async Task<Result> Handle(MakeProductVisibleCommand request, CancellationToken cancellationToken)
    {
        var productResult = await productRepository
            .Load(request.ProductId)
            .ToResult(Errors.ProductNotFound);

        var ownershipResult = productResult
            .Ensure(p => p.VendorId == identifiedUser.Id, Errors.ProductDoesNotBelongToCaller);

        return await ownershipResult
            .Bind(p => p.MakeVisible())
            .Tap(() => productRepository.Store(productResult.Value));
    }
}