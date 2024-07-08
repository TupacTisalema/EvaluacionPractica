using System.Collections.Generic;

namespace OrderProcessingWorkerService.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderItem> Items { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        Processed,
        Failed
    }
}