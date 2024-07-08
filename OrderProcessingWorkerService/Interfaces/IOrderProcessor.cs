using OrderProcessingWorkerService.Models;

namespace OrderProcessingWorkerService.Interfaces
{
    public interface IOrderProcessor
    {
        Task ProcessOrderAsync(Order order, int maxConcurrentItems, CancellationToken cancellationToken);
    }
}