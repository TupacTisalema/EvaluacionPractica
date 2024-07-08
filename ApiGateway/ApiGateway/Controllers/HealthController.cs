using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    [ApiController]
    [Route("api/health")]

    public class HealthController : ControllerBase
    {
        [HttpGet("check")]
        public IActionResult Check()
        {
            var healthStatus = new
            {
                status = "Healthy",
                services = new List<object>
            {
                new { name = "Service1", status = "Healthy" },
                new { name = "Service2", status = "Healthy" }
            }
            };

            return Ok(healthStatus);
        }
    }
}
