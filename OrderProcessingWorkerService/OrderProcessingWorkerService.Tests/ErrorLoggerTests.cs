using Xunit;
using OrderProcessingWorkerService.Models;
using OrderProcessingWorkerService.Services;
using System;

public class ErrorLoggerTests
{
    [Fact]
    public void LogError_LogsException()
    {
        // Arrange
        var logger = new ErrorLogger();
        var order = new Order { Id = 1 };
        var exception = new Exception("Test exception");

        // Act
        logger.LogError(exception, order);

        // Assert
        // Here we are just ensuring that no exceptions are thrown during logging
        Assert.True(true);
    }
}