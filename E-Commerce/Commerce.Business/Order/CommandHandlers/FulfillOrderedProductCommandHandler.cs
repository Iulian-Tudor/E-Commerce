using MediatR;
using CSharpFunctionalExtensions;

using Errors = Commerce.Business.BusinessErrors.OrderedProduct.Fulfill;

namespace Commerce.Business;

internal sealed class FulfillOrderedProductCommandHandler(IIdentifiedUser identifiedUser, IOrderRepository orderRepository, IOrderedProductRepository orderedProductRepository) : IRequestHandler<FulfillOrderedProductCommand, Result>
{
    public async Task<Result> Handle(FulfillOrderedProductCommand request, CancellationToken cancellationToken)
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
            .Bind(() => orderedProductResult.Value.Fulfill())
            .Tap(() => orderedProductRepository.Store(orderedProductResult.Value));
    }
}