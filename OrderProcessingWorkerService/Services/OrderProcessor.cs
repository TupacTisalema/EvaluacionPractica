using OrderProcessingWorkerService.Interfaces;
using OrderProcessingWorkerService.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrderProcessingWorkerService.Services
{
    public class OrderProcessor : IOrderProcessor
    {
        private readonly IErrorLogger _errorLogger;

        public OrderProcessor(IErrorLogger errorLogger)
        {
            _errorLogger = errorLogger;
            OnProcessItemAsync = DefaultProcessItemAsync;
        }

        public Func<OrderItem, CancellationToken, Task> OnProcessItemAsync { get; set; }

        private Task DefaultProcessItemAsync(OrderItem item, CancellationToken cancellationToken)
        {
            // Default implementation for processing item
            Console.WriteLine($"Processing item {item.Name}");
            return Task.CompletedTask;
        }

        public async Task ProcessOrderAsync(Order order, int maxConcurrentItems, CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine($"Starting to process order {order.Id}");
                var itemTasks = order.Items.Take(maxConcurrentItems)
                                           .Select(item => OnProcessItemAsync(item, cancellationToken));

                await Task.WhenAll(itemTasks);
                Console.WriteLine($"Order {order.Id} processed successfully.");
            }
            catch (Exception ex)
            {
                _errorLogger.LogError(ex, order);
                Console.WriteLine($"Error processing order {order.Id}: {ex.Message}");
                throw;
            }
        }
    }
}