using Models;
using Services;
using System.Collections.Concurrent;
using System.Collections.Generic;

public class ActivityLogService : IActivityLogService
{
    private readonly ConcurrentQueue<ActivityLogEntry> _logEntries = new ConcurrentQueue<ActivityLogEntry>();

    public void Log(ActivityLogEntry entry)
    {
        _logEntries.Enqueue(entry);
    }

    public IEnumerable<ActivityLogEntry> GetLogs()
    {
        return _logEntries;
    }
}