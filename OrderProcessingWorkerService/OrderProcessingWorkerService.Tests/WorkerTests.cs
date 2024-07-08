using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OrderProcessingWorkerService;
using OrderProcessingWorkerService.Config;
using OrderProcessingWorkerService.Interfaces;
using OrderProcessingWorkerService.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class WorkerTests
{
    private readonly Mock<ILogger<Worker>> _loggerMock;
    private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
    private readonly Mock<IServiceScope> _serviceScopeMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IOrderProcessor> _orderProcessorMock;
    private readonly Mock<IReportGenerator> _reportGeneratorMock;
    private readonly IOptions<WorkerOptions> _options;
    private readonly Worker _worker;

    public WorkerTests()
    {
        _loggerMock = new Mock<ILogger<Worker>>();
        _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        _serviceScopeMock = new Mock<IServiceScope>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _orderProcessorMock = new Mock<IOrderProcessor>();
        _reportGeneratorMock = new Mock<IReportGenerator>();

        _serviceScopeFactoryMock.Setup(f => f.CreateScope()).Returns(_serviceScopeMock.Object);
        _serviceScopeMock.Setup(s => s.ServiceProvider.GetService(typeof(IOrderRepository))).Returns(_orderRepositoryMock.Object);
        _serviceScopeMock.Setup(s => s.ServiceProvider.GetService(typeof(IOrderProcessor))).Returns(_orderProcessorMock.Object);
        _serviceScopeMock.Setup(s => s.ServiceProvider.GetService(typeof(IReportGenerator))).Returns(_reportGeneratorMock.Object);

        _options = Options.Create(new WorkerOptions
        {
            CronExpression = "*/5 * * * * *",
            MaxConcurrentOrders = 2,
            MaxConcurrentItems = 2
        });

        _worker = new Worker(
            _loggerMock.Object,
            _serviceScopeFactoryMock.Object,
            _options
        );
    }

    [Fact]
    public async Task ExecuteAsync_ProcessesPendingOrders()
    {
        // Arrange
        var pendingOrders = new List<Order>
    {
        new Order { Id = 1, Status = OrderStatus.Pending, Items = new List<OrderItem> { new OrderItem { Id = 1, Name = "Item 1", Price = 10, Quantity = 1 } } },
        new Order { Id = 2, Status = OrderStatus.Pending, Items = new List<OrderItem> { new OrderItem { Id = 2, Name = "Item 2", Price = 20, Quantity = 2 } } }
    };

        _orderRepositoryMock.Setup(repo => repo.GetPendingOrders()).Returns(pendingOrders);
        _orderProcessorMock.Setup(processor => processor.ProcessOrderAsync(It.IsAny<Order>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var cancellationTokenSource = new CancellationTokenSource();
        var workerTask = _worker.StartAsync(cancellationTokenSource.Token);

        // Esperar un tiempo para permitir que el worker procese las órdenes
        await Task.Delay(5000);  // Incrementamos el tiempo de espera para asegurarnos que el proceso se completa
        cancellationTokenSource.Cancel();
        await workerTask;

        // Assert
        _orderRepositoryMock.Verify(repo => repo.UpdateOrderStatus(It.IsAny<Order>(), OrderStatus.Processed), Times.Exactly(pendingOrders.Count), "UpdateOrderStatus was not called the expected number of times.");
        _reportGeneratorMock.Verify(generator => generator.GenerateReport(It.IsAny<IEnumerable<Order>>()), Times.Once, "GenerateReport was not called once as expected.");
    }
}