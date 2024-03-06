using MediatR;
using Commerce.Domain;
using CSharpFunctionalExtensions;

namespace Commerce.Business;

internal sealed class PlaceOrderCommandHandler(IOrderRepository orderRepository, IProductRepository productRepository, IUserRepository userRepository) : IRequestHandler<PlaceOrderCommand, Result>
{
    public async Task<Result> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var userResult = await userRepository
            .Load(request.ClientId)
            .ToResult(BusinessErrors.Order.Create.UserNotFound);

        var products = productRepository
            .Query()
            .Where(p => request.Products.Contains(p.Id))
            .Where(p => p.IsVisible)
            .ToList()
            .Select(p => new OrderedProduct(p.Id, p.CategoryId, p.VendorId, request.ClientId, p.VendorName, p.Name, p.Description, p.Price, request.Products.Count(r => r == p.Id), p.IsVisible))
            .ToList();

        var productsResult = Result.SuccessIf(request.Products.All(products.Select(p => p.ProductId).Contains), BusinessErrors.Order.Create.ProductNotFound);

        return await Result
            .FirstFailureOrSuccess(userResult, productsResult)
            .Bind(() => Order.Create(request.ClientId, $"{userResult.Value.FirstName} {userResult.Value.LastName}", request.ShippingAddress, products, products.Sum(p => p.TotalPrice)))
            .Tap(o =>
            {
                foreach (var orderedProduct in o.Products)
                {
                    orderedProduct.SetOrderId(o.Id);
                    orderedProduct.SetShippingAddress(o.ShippingAddress);
                }
            })
            .Tap(orderRepository.Store);
    }
}