using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace ApiGateway.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    [ApiController]
    [Route("api/activity-log")]
    public class ActivityLogController : ControllerBase
    {
        private readonly IActivityLogService _activityLogService;

        public ActivityLogController(IActivityLogService activityLogService)
        {
            _activityLogService = activityLogService;
        }

        [HttpGet]
        public IEnumerable<ActivityLogEntry> Get()
        {
            return _activityLogService.GetLogs();
        }
    }
}
