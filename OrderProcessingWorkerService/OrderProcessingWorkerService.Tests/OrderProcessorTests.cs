using OrderProcessingWorkerService.Interfaces;
using OrderProcessingWorkerService.Models;
using OrderProcessingWorkerService.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class OrderProcessorTests
{
    private readonly Mock<IErrorLogger> _errorLoggerMock;
    private readonly OrderProcessor _orderProcessor;

    public OrderProcessorTests()
    {
        _errorLoggerMock = new Mock<IErrorLogger>();
        _orderProcessor = new OrderProcessor(_errorLoggerMock.Object);
    }

    [Fact]
    public async Task ProcessOrderAsync_ProcessesItemsSuccessfully()
    {
        // Arrange
        var order = new Order
        {
            Id = 1,
            Items = new List<OrderItem>
            {
                new OrderItem { Id = 1, Name = "Item 1", Price = 10, Quantity = 1 },
                new OrderItem { Id = 2, Name = "Item 2", Price = 20, Quantity = 2 }
            }
        };

        // Act
        await _orderProcessor.ProcessOrderAsync(order, 2, CancellationToken.None);

        // Assert
        _errorLoggerMock.Verify(logger => logger.LogError(It.IsAny<Exception>(), order), Times.Never);
    }

    [Fact]
    public async Task ProcessOrderAsync_LogsErrorOnException()
    {
        // Arrange
        var order = new Order
        {
            Id = 1,
            Items = new List<OrderItem>
            {
                new OrderItem { Id = 1, Name = "Item 1", Price = 10, Quantity = 1 },
                new OrderItem { Id = 2, Name = "Item 2", Price = 20, Quantity = 2 }
            }
        };

        _orderProcessor.OnProcessItemAsync = (item, token) => throw new Exception("Test exception");

        // Act
        await Assert.ThrowsAsync<Exception>(() => _orderProcessor.ProcessOrderAsync(order, 2, CancellationToken.None));

        // Assert
        _errorLoggerMock.Verify(logger => logger.LogError(It.IsAny<Exception>(), order), Times.Once);
    }
}