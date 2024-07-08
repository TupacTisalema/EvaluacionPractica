using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace ApiGateway.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    [Route("api/gateway")]
    [ApiController]
    public class GatewayController : ControllerBase
    {
        private readonly IApiGatewayService _apiGatewayService;

        public GatewayController(IApiGatewayService apiGatewayService)
        {
            _apiGatewayService = apiGatewayService;
        }

        [HttpPost("forward")]
        public async Task<IActionResult> ForwardRequest([FromBody] GatewayRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _apiGatewayService.ForwardRequestAsync(request);
            return Ok(response);
        }
    }
}
