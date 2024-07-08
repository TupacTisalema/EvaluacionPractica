using Models;

namespace Services
{
    public interface IActivityLogService
    {
        void Log(ActivityLogEntry entry);
        IEnumerable<ActivityLogEntry> GetLogs();
    }
}
