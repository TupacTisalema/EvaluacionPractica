using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Services;
using Models;
using System.Diagnostics;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;
    private readonly IActivityLogService _activityLogService;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger, IActivityLogService activityLogService)
    {
        _next = next;
        _logger = logger;
        _activityLogService = activityLogService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation($"Incoming request: {context.Request.Method} {context.Request.Path}");

        await _next(context);

        stopwatch.Stop();
        var statusCode = context.Response.StatusCode;
        var method = context.Request.Method;
        var url = context.Request.Path;

        _logger.LogInformation($"Outgoing response: {statusCode}");

        var logEntry = new ActivityLogEntry
        {
            Timestamp = DateTime.UtcNow,
            Method = method,
            Url = url,
            StatusCode = statusCode,
            ResponseTime = stopwatch.ElapsedMilliseconds
        };

        _activityLogService.Log(logEntry);
    }
}