using OrderProcessingWorkerService.Interfaces;
using OrderProcessingWorkerService.Models;
using OrderProcessingWorkerService.Services;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class OrderRepositoryTests
{
    private readonly OrderRepository _orderRepository;

    public OrderRepositoryTests()
    {
        _orderRepository = new OrderRepository();
    }

    [Fact]
    public void GetPendingOrders_ReturnsPendingOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order { Id = 1, Status = OrderStatus.Pending },
            new Order { Id = 2, Status = OrderStatus.Processed }
        };
        _orderRepository.SetOrders(orders);

        // Act
        var pendingOrders = _orderRepository.GetPendingOrders();

        // Assert
        Assert.Single(pendingOrders);
        Assert.Equal(1, pendingOrders.First().Id);
    }

    [Fact]
    public void UpdateOrderStatus_UpdatesStatus()
    {
        // Arrange
        var order = new Order { Id = 1, Status = OrderStatus.Pending };
        var orders = new List<Order> { order };
        _orderRepository.SetOrders(orders);

        // Act
        _orderRepository.UpdateOrderStatus(order, OrderStatus.Processed);

        // Assert
        Assert.Equal(OrderStatus.Processed, order.Status);
    }
}