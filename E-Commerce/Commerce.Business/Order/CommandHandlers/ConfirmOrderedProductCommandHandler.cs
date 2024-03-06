using MediatR;
using CSharpFunctionalExtensions;

using Errors = Commerce.Business.BusinessErrors.OrderedProduct.Confirm;

namespace Commerce.Business;

internal sealed class ConfirmOrderedProductCommandHandler(IIdentifiedUser identifiedUser, IOrderRepository orderRepository, IOrderedProductRepository orderedProductRepository) : IRequestHandler<ConfirmOrderedProductCommand, Result>
{
    public async Task<Result> Handle(ConfirmOrderedProductCommand request, CancellationToken cancellationToken)
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
            .FirstFailureOrSuccess(orderResult, ownershipResult, orderedProductResult)
            .Bind(() => orderedProductResult.Value.Confirm())
            .Tap(() => orderedProductRepository.Store(orderedProductResult.Value));
    }
}