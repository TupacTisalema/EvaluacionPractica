using OrderProcessingWorkerService.Models;

namespace OrderProcessingWorkerService.Interfaces
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetPendingOrders();
        void UpdateOrderStatus(Order order, OrderStatus status);
    }
}
