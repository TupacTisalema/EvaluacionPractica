using OrderProcessingWorkerService.Interfaces;
using OrderProcessingWorkerService.Models;
using System;

namespace OrderProcessingWorkerService.Services
{
    public class ErrorLogger : IErrorLogger
    {
        public void LogError(Exception ex, Order order)
        {
            // Lógica para registrar el error
            Console.WriteLine($"Error processing order {order.Id}: {ex.Message}");
        }
    }
}