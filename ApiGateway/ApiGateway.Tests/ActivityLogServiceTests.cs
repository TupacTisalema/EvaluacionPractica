using Xunit;
using System.Linq;
using System;
using Models;
using Services;

public class ActivityLogServiceTests
{
    private readonly IActivityLogService _activityLogService;

    public ActivityLogServiceTests()
    {
        _activityLogService = new ActivityLogService();
    }

    [Fact]
    public void Log_ShouldAddEntry()
    {
        // Arrange
        var entry = new ActivityLogEntry
        {
            Timestamp = DateTime.UtcNow,
            Method = "GET",
            Url = "http://example.com",
            StatusCode = 200,
            ResponseTime = 100
        };

        // Act
        _activityLogService.Log(entry);

        // Assert
        var logs = _activityLogService.GetLogs();
        Assert.Contains(entry, logs);
    }

    [Fact]
    public void GetLogs_ShouldReturnEntries()
    {
        // Arrange
        var entry1 = new ActivityLogEntry
        {
            Timestamp = DateTime.UtcNow,
            Method = "GET",
            Url = "http://example.com/1",
            StatusCode = 200,
            ResponseTime = 100
        };

        var entry2 = new ActivityLogEntry
        {
            Timestamp = DateTime.UtcNow,
            Method = "POST",
            Url = "http://example.com/2",
            StatusCode = 201,
            ResponseTime = 150
        };

        _activityLogService.Log(entry1);
        _activityLogService.Log(entry2);

        // Act
        var logs = _activityLogService.GetLogs();

        // Assert
        Assert.Equal(2, logs.Count());
        Assert.Contains(entry1, logs);
        Assert.Contains(entry2, logs);
    }
}