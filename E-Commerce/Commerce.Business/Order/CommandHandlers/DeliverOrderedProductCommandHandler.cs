using MediatR;
using CSharpFunctionalExtensions;

using Errors = Commerce.Business.BusinessErrors.OrderedProduct.Deliver;

namespace Commerce.Business;

internal sealed class DeliverOrderedProductCommandHandler(IIdentifiedUser identifiedUser, IOrderRepository orderRepository, IOrderedProductRepository orderedProductRepository) : IRequestHandler<DeliverOrderedProductCommand, Result>
{
    public async Task<Result> Handle(DeliverOrderedProductCommand request, CancellationToken cancellationToken)
    {
        var orderResult = await orderRepository
            .Load(request.OrderId)
            .ToResult(Errors.OrderNotFound);

        var orderedProductResult = await orderedProductRepository
            .Load(request.OrderedProductId)
            .ToResult(Errors.OrderedProductNotFound);

        var ownershipResult = orderedProductResult
            .Ensure(op => op.VendorId == identifiedUser.Id, Errors.OrderedProductNotSoldByCaller);

        return await Result
            .FirstFailureOrSuccess(orderResult, ownershipResult)
            .Bind(() => orderedProductResult.Value.Deliver())
            .Tap(() => orderedProductRepository.Store(orderedProductResult.Value));
    }
}