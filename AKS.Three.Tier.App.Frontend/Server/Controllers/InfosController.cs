using AKS.Three.Tier.App.Frontend.Shared;
using ApiCustomNamespace;
using Microsoft.AspNetCore.Mvc;

namespace AKS.Three.Tier.App.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InfosController : ControllerBase
    {
        private readonly ILogger<InfosController> _logger;
        private readonly ApiClient _client;

        public InfosController(ILogger<InfosController> logger, ApiCustomNamespace.ApiClient apiClient)
        {
            _logger = logger;
            this._client = apiClient;
        }

        public async Task<CompleteRequestInfosDTO> Get()
        {
            FrontendEnvironmentInfosDTO environmentInfosDTO = new();
            await environmentInfosDTO.GetIpInfosAsync();
            var apiInfosResult = await _client.GetEnvironmentInfosAsync();

            var finalResult = new CompleteRequestInfosDTO()
            {
                FrontendInfos = environmentInfosDTO,
                ApiInfos = Map(apiInfosResult)
            };

            return finalResult;
        }

        private APIEnvInfosDTO Map(APIEnvironmentInfosDTO obj)
        {
            return new APIEnvInfosDTO()
            {
                Cgroup = obj.Cgroup,
                Containerized = obj.Containerized,
                FrameworkDescription = obj.FrameworkDescription,
                HostName = obj.HostName,
                IpList = obj.IpList,
                Limit = obj.Limit,
                MemoryLimit = obj.MemoryLimit,
                MemoryUsage = obj.MemoryUsage,
                OsArchitecture = obj.OsArchitecture,
                OsDescription = obj.OsDescription,
                ProcessorCount = obj.ProcessorCount,
                TotalAvailableMemory = obj.TotalAvailableMemory,
                Usage = obj.Usage,
                DbEntities = Map(obj.DbEntities)
            };
        }

        private ICollection<ClientDbEntity> Map(ICollection<DbEntity> dbEntities)
        {
            return dbEntities.Select(x => new ClientDbEntity()
            {
                HostName = x.HostName,
                CreationDate = x.CreationDate
            }).ToList();
        }
    }
}