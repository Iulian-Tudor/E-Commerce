using MediatR;
using CSharpFunctionalExtensions;

using Errors = Commerce.Business.BusinessErrors.Product.ChangeCategory;

namespace Commerce.Business;

internal sealed class ChangeProductCategoryCommandHandler(IProductRepository productRepository, ICategoryRepository categoryRepository, IIdentifiedUser identifiedUser) : IRequestHandler<ChangeProductCategoryCommand, Result>
{
    public async Task<Result> Handle(ChangeProductCategoryCommand request, CancellationToken cancellationToken)
    {
        var categoryResult = await categoryRepository
            .Load(request.CategoryId)
            .ToResult(Errors.CategoryNotFound);

        var productResult = await productRepository
            .Load(request.ProductId)
            .ToResult(Errors.ProductNotFound);

        var ownershipResult = productResult
            .Ensure(p => p.VendorId == identifiedUser.Id, Errors.ProductDoesNotBelongToCaller);

        return await Result
            .FirstFailureOrSuccess(categoryResult, productResult, ownershipResult)
            .Bind(() => productResult.Value.ChangeCategory(request.CategoryId))
            .Tap(() => productRepository.Store(productResult.Value));
    }
}