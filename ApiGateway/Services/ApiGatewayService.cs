using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Models;
using Services;

public class ApiGatewayService : IApiGatewayService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ApiGatewayService> _logger;

    public ApiGatewayService(IHttpClientFactory httpClientFactory, ILogger<ApiGatewayService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<GatewayResponse> ForwardRequestAsync(GatewayRequest request)
    {
        try
        {
            _logger.LogInformation("Forwarding request to {Url} with method {Method}", request.Url, request.Method);

            var client = _httpClientFactory.CreateClient();
            var httpRequest = new HttpRequestMessage
            {
                Method = new HttpMethod(request.Method),
                RequestUri = new Uri(request.Url),
                Content = string.IsNullOrEmpty(request.Body) ? null : new StringContent(request.Body, Encoding.UTF8, "application/json")
            };

            foreach (var header in request.Headers)
            {
                if (header.Key.ToLower() == "content-type" && httpRequest.Content != null)
                {
                    httpRequest.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(header.Value);
                }
                else
                {
                    httpRequest.Headers.Add(header.Key, header.Value);
                }
            }

            _logger.LogInformation("Sending request to {Url}", request.Url);
            var response = await client.SendAsync(httpRequest);
            var responseBody = await response.Content.ReadAsStringAsync();
            var responseHeaders = response.Headers.ToDictionary(h => h.Key, h => string.Join(", ", h.Value));

            _logger.LogInformation("Received response with status code {StatusCode}", (int)response.StatusCode);

            // Deserializar el JSON del cuerpo de la respuesta
            object deserializedBody;
            string formattedBody;
            try
            {
                deserializedBody = JsonSerializer.Deserialize<object>(responseBody);
                formattedBody = JsonSerializer.Serialize(deserializedBody, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (JsonException)
            {
                formattedBody = responseBody; // Si no se puede deserializar, mantenerlo como string
            }

            return new GatewayResponse
            {
                StatusCode = (int)response.StatusCode,
                Body = formattedBody,
                Headers = responseHeaders
            };
        }
        catch (HttpRequestException e)
        {
            _logger.LogError($"Error forwarding request: {e.Message}");
            return new GatewayResponse
            {
                StatusCode = 500,
                Body = $"Error: {e.Message}",
                Headers = new Dictionary<string, string>()
            };
        }
    }
}