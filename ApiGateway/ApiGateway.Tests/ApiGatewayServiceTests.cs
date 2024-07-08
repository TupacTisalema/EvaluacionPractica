using Xunit;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;
using Models;
using Services;

public class ApiGatewayServiceTests
{
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly IApiGatewayService _apiGatewayService;
    private readonly Mock<ILogger<ApiGatewayService>> _mockLogger;

    public ApiGatewayServiceTests()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockLogger = new Mock<ILogger<ApiGatewayService>>();
        _apiGatewayService = new ApiGatewayService(_mockHttpClientFactory.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task ForwardRequestAsync_ShouldReturnResponse_WhenRequestIsValid()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"key\":\"value\"}")
            });

        var client = new HttpClient(mockHttpMessageHandler.Object);
        _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

        var request = new GatewayRequest
        {
            Method = "POST",
            Url = "http://example.com",
            Body = "{\"data\":\"test\"}",
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" }
            }
        };

        // Act
        var response = await _apiGatewayService.ForwardRequestAsync(request);

        // Assert
        var expectedResponseBody = JsonSerializer.Serialize(new { key = "value" }, new JsonSerializerOptions { WriteIndented = true });
        Assert.Equal(200, response.StatusCode);
        Assert.Equal(expectedResponseBody, response.Body);
    }

    [Fact]
    public async Task ForwardRequestAsync_ShouldReturnErrorResponse_WhenExceptionIsThrown()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Network error"));

        var client = new HttpClient(mockHttpMessageHandler.Object);
        _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

        var request = new GatewayRequest
        {
            Method = "GET",
            Url = "http://example.com",
            Body = "",
            Headers = new Dictionary<string, string>()
        };

        // Act
        var response = await _apiGatewayService.ForwardRequestAsync(request);

        // Assert
        Assert.Equal(500, response.StatusCode);
        Assert.StartsWith("Error:", response.Body);
    }
}