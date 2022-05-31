using Microsoft.AspNetCore.Mvc;

namespace AKS.Three.Tier.App.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InfosController : ControllerBase
    {
        private readonly ILogger<InfosController> _logger;

        public InfosController(ILogger<InfosController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetEnvironmentInfos")]
        public async Task<EnvironmentInfosDTO> Get()
        {
            EnvironmentInfosDTO environmentInfosDTO = new EnvironmentInfosDTO();
            await environmentInfosDTO.GetIpInfosAsync();
            return environmentInfosDTO;
        }
    }
}