using MediatR;
using CSharpFunctionalExtensions;

using Errors = Commerce.Business.BusinessErrors.Product.ChangeDetails;

namespace Commerce.Business;

internal sealed class ChangeProductDetailsCommandHandler(IProductRepository repository, IIdentifiedUser identifiedUser) : IRequestHandler<ChangeProductDetailsCommand, Result>
{
    public async Task<Result> Handle(ChangeProductDetailsCommand request, CancellationToken cancellationToken)
    {
        var productResult = await repository
            .Load(request.ProductId)
            .ToResult(Errors.ProductNotFound);

        var ownershipResult = productResult
            .Ensure(p => p.VendorId == identifiedUser.Id, Errors.ProductDoesNotBelongToCaller);

        return await ownershipResult
            .Check(p => p.ChangeDetails(request.Name, request.Description, request.Price))
            .Tap(repository.Store);
    }
}