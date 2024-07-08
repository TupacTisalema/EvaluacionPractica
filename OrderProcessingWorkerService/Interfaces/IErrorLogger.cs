using OrderProcessingWorkerService.Models;

namespace OrderProcessingWorkerService.Interfaces
{
    public interface IErrorLogger
    {
        void LogError(Exception ex, Order order);
    }
}
