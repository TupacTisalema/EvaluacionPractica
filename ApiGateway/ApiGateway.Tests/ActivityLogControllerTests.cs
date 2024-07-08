using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ApiGateway.Controllers;
using Models;
using Services;

public class ActivityLogControllerTests
{
    private readonly Mock<IActivityLogService> _mockActivityLogService;
    private readonly ActivityLogController _controller;

    public ActivityLogControllerTests()
    {
        _mockActivityLogService = new Mock<IActivityLogService>();
        _controller = new ActivityLogController(_mockActivityLogService.Object);
    }

    [Fact]
    public void Get_ShouldReturnLogs()
    {
        // Arrange
        var logs = new List<ActivityLogEntry>
        {
            new ActivityLogEntry { Method = "GET", Url = "http://example.com", StatusCode = 200, ResponseTime = 100 }
        };
        _mockActivityLogService.Setup(service => service.GetLogs()).Returns(logs);

        // Act
        var result = _controller.Get() as IEnumerable<ActivityLogEntry>;

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("GET", result.First().Method);
    }
}