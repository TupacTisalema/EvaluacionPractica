using OrderProcessingWorkerService.Interfaces;
using OrderProcessingWorkerService.Models;
using System.Collections.Generic;
using System.Linq;

namespace OrderProcessingWorkerService.Services
{
    public class OrderRepository : IOrderRepository
    {
        private List<Order> _orders = new List<Order>();

        public IEnumerable<Order> GetPendingOrders()
        {
            return _orders.Where(order => order.Status == OrderStatus.Pending).ToList();
        }

        public void UpdateOrderStatus(Order order, OrderStatus status)
        {
            var existingOrder = _orders.FirstOrDefault(o => o.Id == order.Id);
            if (existingOrder != null)
            {
                existingOrder.Status = status;
                Console.WriteLine($"Order {order.Id} status updated to {status}");
            }
        }

        public void SetOrders(List<Order> orders)
        {
            _orders = orders;
            Console.WriteLine("Orders set in repository");
        }
    }
}