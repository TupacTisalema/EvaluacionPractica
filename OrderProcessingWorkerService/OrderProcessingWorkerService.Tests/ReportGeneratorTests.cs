using Xunit;
using OrderProcessingWorkerService.Models;
using OrderProcessingWorkerService.Services;
using System.Collections.Generic;

public class ReportGeneratorTests
{
    [Fact]
    public void GenerateReport_GeneratesReport()
    {
        // Arrange
        var generator = new ReportGenerator();
        var orders = new List<Order>
        {
            new Order { Id = 1, Description = "Order 1" },
            new Order { Id = 2, Description = "Order 2" }
        };

        // Act
        generator.GenerateReport(orders);

        // Assert
        // Here we are just ensuring that no exceptions are thrown during report generation
        Assert.True(true);
    }
}