using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderProcessingWorkerService.Config;
using OrderProcessingWorkerService.Interfaces;
using OrderProcessingWorkerService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cronos;

namespace OrderProcessingWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly CronExpression _cronExpression;
        private readonly int _maxConcurrentOrders;
        private readonly int _maxConcurrentItems;
        private DateTime _nextRunTime;

        public Worker(
            ILogger<Worker> logger,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<WorkerOptions> options)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            var workerOptions = options.Value;
            _cronExpression = CronExpression.Parse(workerOptions.CronExpression, CronFormat.IncludeSeconds);
            _maxConcurrentOrders = workerOptions.MaxConcurrentOrders;
            _maxConcurrentItems = workerOptions.MaxConcurrentItems;
            _nextRunTime = _cronExpression.GetNextOccurrence(DateTime.UtcNow) ?? DateTime.UtcNow.AddMinutes(1);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (DateTime.UtcNow >= _nextRunTime)
                {
                    _logger.LogInformation("Worker running at: {time}", DateTime.UtcNow);

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                        var orderProcessor = scope.ServiceProvider.GetRequiredService<IOrderProcessor>();
                        var reportGenerator = scope.ServiceProvider.GetRequiredService<IReportGenerator>();

                        var pendingOrders = orderRepository.GetPendingOrders();
                        _logger.LogInformation("Found {OrderCount} pending orders.", pendingOrders.Count());

                        var tasks = pendingOrders.Take(_maxConcurrentOrders)
                                                 .Select(order => ProcessOrderAsync(orderProcessor, orderRepository, order, stoppingToken));

                        await Task.WhenAll(tasks);
                        reportGenerator.GenerateReport(pendingOrders);
                    }

                    _nextRunTime = _cronExpression.GetNextOccurrence(DateTime.UtcNow) ?? DateTime.UtcNow.AddMinutes(1);
                }

                await Task.Delay(1000, stoppingToken); // Polling interval, adjust as necessary
            }
        }

        private async Task ProcessOrderAsync(IOrderProcessor orderProcessor, IOrderRepository orderRepository, Order order, CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Processing order {OrderId}", order.Id);
                await orderProcessor.ProcessOrderAsync(order, _maxConcurrentItems, stoppingToken);
                _logger.LogInformation("Order {OrderId} processed successfully", order.Id);
                orderRepository.UpdateOrderStatus(order, OrderStatus.Processed);  // Asegúrate de que esta línea se ejecuta
                _logger.LogInformation("Order {OrderId} status updated to Processed", order.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing order {OrderId}", order.Id);
                orderRepository.UpdateOrderStatus(order, OrderStatus.Failed);  // Asegúrate de que esta línea se ejecuta
                _logger.LogInformation("Order {OrderId} status updated to Failed", order.Id);
            }
        }
    }
}