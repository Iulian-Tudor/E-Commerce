using Xunit;
using NSubstitute;
using Commerce.Domain;
using FluentAssertions;

namespace Commerce.Business.Tests;

public sealed class PlaceOrderCommandHandlerTests
{
    private static readonly ProductReadModel FirstProduct = new ProductReadModel().FromAggregate(ProductsFactory.Any().Visible());
    private static readonly ProductReadModel SecondProduct = new ProductReadModel().FromAggregate(ProductsFactory.Any().Visible());
    private static readonly IReadOnlyCollection<Guid> ProductIds = new[] { FirstProduct.Id, FirstProduct.Id, FirstProduct.Id, SecondProduct.Id, SecondProduct.Id };

    private readonly IOrderRepository orderRepository = Substitute.For<IOrderRepository>();
    private readonly IProductRepository productRepository = Substitute.For<IProductRepository>();
    private readonly IUserRepository userRepository = Substitute.For<IUserRepository>();

    private readonly User user = UsersFactory.Any();
    private readonly IReadOnlyCollection<ProductReadModel> products = new[] { FirstProduct, SecondProduct };

    public PlaceOrderCommandHandlerTests()
    {
        userRepository.Load(user.Id).Returns(user);
        productRepository.Query().Returns(products.AsQueryable());
    }

    [Fact]
    public async Task When_UserNotFound_Then_ShouldFail()
    {
        // Arrange
        var command = Command() with { ClientId = Guid.NewGuid() };

        // Act
        var result = await Sut().Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BusinessErrors.Order.Create.UserNotFound);

        await orderRepository.DidNotReceive().Store(Arg.Any<Order>());
    }

    [Fact]
    public async Task When_ProductNotFound_Then_ShouldFail()
    {
        // Arrange
        var command = Command() with { Products = new[] { Guid.NewGuid() } };

        // Act
        var result = await Sut().Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BusinessErrors.Order.Create.ProductNotFound);

        await orderRepository.DidNotReceive().Store(Arg.Any<Order>());
    }

    [Fact]
    public async Task When_NotViolatingConstraints_Then_ShouldSucceed()
    {
        // Act
        var result = await Sut().Handle(Command(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await orderRepository.Received(1).Store(Arg.Any<Order>());

        var order = orderRepository.ReceivedCalls().Single().GetArguments()[0] as Order;
        order.Should().NotBeNull();

        order.ClientId.Should().Be(user.Id);
        order.Products.Should().HaveCount(2);
        order.Products.Should().Contain(p => p.ProductId == FirstProduct.Id && p.Count == 3);
        order.Products.Should().Contain(p => p.ProductId == SecondProduct.Id && p.Count == 2);
    }

    private PlaceOrderCommand Command() => new(user.Id, "address", ProductIds);

    private PlaceOrderCommandHandler Sut() => new(orderRepository, productRepository, userRepository);
}