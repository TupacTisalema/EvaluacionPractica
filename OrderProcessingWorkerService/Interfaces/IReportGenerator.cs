using OrderProcessingWorkerService.Models;

namespace OrderProcessingWorkerService.Interfaces
{
    public interface IReportGenerator
    {
        void GenerateReport(IEnumerable<Order> orders);
    }
}