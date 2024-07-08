using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using OrderProcessingWorkerService;
using OrderProcessingWorkerService.Interfaces;
using OrderProcessingWorkerService.Models;
using OrderProcessingWorkerService.Config;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using OrderProcessingWorkerService.Services;

public class IntegrationTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<WorkerOptions> _options;

    public IntegrationTests()
    {
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddScoped<IOrderProcessor, OrderProcessor>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IErrorLogger, ErrorLogger>();
        services.AddScoped<IReportGenerator, ReportGenerator>();
        services.AddSingleton<IHostedService, Worker>();

        // Configuración de WorkerOptions desde appsettings.json
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        services.Configure<WorkerOptions>(configuration.GetSection("WorkerOptions"));

        _serviceProvider = services.BuildServiceProvider();
        _options = _serviceProvider.GetRequiredService<IOptions<WorkerOptions>>();
    }

    [Fact]
    public async Task Worker_ProcessesOrdersSuccessfully()
    {
        // Arrange
        var logger = _serviceProvider.GetRequiredService<ILogger<Worker>>();
        var orderRepository = _serviceProvider.GetRequiredService<IOrderRepository>();
        var orderProcessor = _serviceProvider.GetRequiredService<IOrderProcessor>();
        var reportGenerator = _serviceProvider.GetRequiredService<IReportGenerator>();
        var worker = new Worker(
            logger,
            _serviceProvider.GetRequiredService<IServiceScopeFactory>(),
            _options
        );

        var pendingOrders = new List<Order>
        {
            new Order { Id = 1, Status = OrderStatus.Pending, Items = new List<OrderItem> { new OrderItem { Id = 1, Name = "Item 1", Price = 10, Quantity = 1 } } },
            new Order { Id = 2, Status = OrderStatus.Pending, Items = new List<OrderItem> { new OrderItem { Id = 2, Name = "Item 2", Price = 20, Quantity = 2 } } }
        };

        ((OrderRepository)orderRepository).SetOrders(pendingOrders);

        // Act
        var cancellationTokenSource = new CancellationTokenSource();
        var workerTask = worker.StartAsync(cancellationTokenSource.Token);

        // Esperar un tiempo para permitir que el worker procese las órdenes
        await Task.Delay(10000); // Incrementamos el tiempo de espera para asegurarnos que el proceso se completa
        cancellationTokenSource.Cancel();
        await workerTask;

        // Assert
        foreach (var order in pendingOrders)
        {
            Console.WriteLine($"Order {order.Id} status: {order.Status}");
            Assert.Equal(OrderStatus.Processed, order.Status);
        }

        // Verificar que se haya generado el reporte
        var report = ((ReportGenerator)reportGenerator).GetGeneratedReport();
        Assert.NotNull(report);
        Assert.Equal(pendingOrders.Count, report.Count);
    }
}