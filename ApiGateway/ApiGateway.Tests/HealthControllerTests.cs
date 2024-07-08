using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ApiGateway.Controllers;

public class HealthControllerTests
{
    private readonly HealthController _controller;

    public HealthControllerTests()
    {
        _controller = new HealthController();
    }

    [Fact]
    public void Get_ShouldReturnHealthyStatus()
    {
        // Act
        var result = _controller.Check() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

        var healthStatusJson = JsonSerializer.Serialize(result.Value);
        var healthStatus = JsonSerializer.Deserialize<HealthStatus>(healthStatusJson);

        Assert.Equal("Healthy", healthStatus.status);
        Assert.All(healthStatus.services, service => Assert.Equal("Healthy", service.status));
    }

    private class HealthStatus
    {
        public string status { get; set; }
        public ServiceStatus[] services { get; set; }
    }

    private class ServiceStatus
    {
        public string name { get; set; }
        public string status { get; set; }
    }
}