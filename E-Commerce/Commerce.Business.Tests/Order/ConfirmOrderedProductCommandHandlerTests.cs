using Xunit;
using NSubstitute;
using Commerce.Domain;
using FluentAssertions;

namespace Commerce.Business.Tests;

public sealed class ConfirmOrderedProductCommandHandlerTests
{
    private IIdentifiedUser identifiedUser = TestableUser.Any();
    private readonly IOrderedProductRepository orderedProductRepository = Substitute.For<IOrderedProductRepository>();
    private readonly IOrderRepository orderRepository = Substitute.For<IOrderRepository>();

    private readonly OrderedProduct orderedProduct;
    private readonly Order order;

    public ConfirmOrderedProductCommandHandlerTests()
    {
        orderedProduct = OrderedProductsFactory.WithIds(Guid.NewGuid(), Guid.NewGuid(), identifiedUser.Id);
        order = OrdersFactory.WithDetails(Guid.NewGuid(), new List<OrderedProduct> { orderedProduct });

        orderedProductRepository.Load(orderedProduct.Id).Returns(orderedProduct);
        orderRepository.Load(order.Id).Returns(order);
    }

    [Fact]
    public async Task When_OrderNotFound_Then_ShouldFail()
    {
        // Arrange
        var command = Command() with { OrderId = Guid.NewGuid() };

        // Act
        var result = await Sut().Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BusinessErrors.OrderedProduct.Confirm.OrderNotFound);
        
        await orderedProductRepository.DidNotReceive().Store(Arg.Any<OrderedProduct>());
    }

    [Fact]
    public async Task When_OrderedProductNotFound_Then_ShouldFail()
    {
        // Arrange
        var command = Command() with { OrderedProductId = Guid.NewGuid() };

        // Act
        var result = await Sut().Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BusinessErrors.OrderedProduct.Confirm.OrderedProductNotFound);

        await orderedProductRepository.DidNotReceive().Store(Arg.Any<OrderedProduct>());
    }

    [Fact]
    public async Task When_CallerIsNotSeller_Then_ShouldFail()
    {
        // Arrange
        identifiedUser = TestableUser.Any();

        // Act
        var result = await Sut().Handle(Command(), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BusinessErrors.OrderedProduct.Confirm.OrderedProductNotSoldByCaller);

        await orderedProductRepository.DidNotReceive().Store(Arg.Any<OrderedProduct>());
    }

    [Fact]
    public async Task When_DomainFails_Then_ShouldFail()
    {
        // Arrange
        orderedProduct.Fulfilled();

        // Act
        var result = await Sut().Handle(Command(), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.OrderedProduct.Confirm.NotPending);

        await orderedProductRepository.DidNotReceive().Store(Arg.Any<OrderedProduct>());
    }

    [Fact]
    public async Task When_DomainSucceeds_Then_ShouldSucceed()
    {
        // Act
        var result = await Sut().Handle(Command(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await orderedProductRepository.Received(1).Store(orderedProduct);
    }

    private ConfirmOrderedProductCommand Command() => new(order.Id, orderedProduct.Id);

    private ConfirmOrderedProductCommandHandler Sut() => new(identifiedUser, orderRepository, orderedProductRepository);
}