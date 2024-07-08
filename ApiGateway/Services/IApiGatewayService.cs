using Models;

namespace Services
{
    public interface IApiGatewayService
    {
        Task<GatewayResponse> ForwardRequestAsync(GatewayRequest request);
    }
}
