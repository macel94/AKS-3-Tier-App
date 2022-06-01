using Microsoft.AspNetCore.Mvc;

namespace AKS.Three.Tier.App.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiInfosController : ControllerBase
    {
        private readonly ILogger<ApiInfosController> _logger;

        public ApiInfosController(ILogger<ApiInfosController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetEnvironmentInfos")]
        public async Task<APIEnvironmentInfosDTO> Get()
        {
            APIEnvironmentInfosDTO environmentInfosDTO = new();
            await environmentInfosDTO.GetIpInfosAsync();
            return environmentInfosDTO;
        }
    }
}