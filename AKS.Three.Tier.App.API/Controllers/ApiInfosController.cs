using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;

namespace AKS.Three.Tier.App.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiInfosController : ControllerBase
    {
        private readonly ILogger<ApiInfosController> _logger;
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public ApiInfosController(ILogger<ApiInfosController> logger, ConnectionMultiplexer redis)
        {
            _logger = logger;
            _redis = redis;
            _database = redis.GetDatabase();
        }

        [HttpGet(Name = "GetEnvironmentInfos")]
        public async Task<APIEnvironmentInfosDTO> Get()
        {
            APIEnvironmentInfosDTO environmentInfosDTO = new();
            await environmentInfosDTO.GetIpInfosAsync();
            var currentResults = await GetUpdatedDbEntitiesAsync(environmentInfosDTO);
            environmentInfosDTO.DbEntities = currentResults;
            return environmentInfosDTO;
        }

        private async Task<List<DbEntity?>?> GetUpdatedDbEntitiesAsync(APIEnvironmentInfosDTO currentInfos)
        {
            var random = new Random();

            await Task.Delay(random.Next(10, 300));
            var newEntity = new DbEntity()
            {
                HostName = currentInfos.HostName,
                CreationDate = DateTimeOffset.UtcNow
            };

            var totalItemsAfterPush = await _database.ListLeftPushAsync("list-of-entities", JsonSerializer.Serialize(newEntity));
            if (totalItemsAfterPush <= 0)
            {
                return null;
            }
            if (totalItemsAfterPush > 10)
            {
                await _database.ListRightPopAsync("list-of-entities");
            }
            var currentSet = await _database.ListRangeAsync("list-of-entities", 0, 9);

            return currentSet.Select(x => JsonSerializer.Deserialize<DbEntity>(x)).ToList();
        }

        private IServer GetServer()
        {
            var endpoint = _redis.GetEndPoints();
            return _redis.GetServer(endpoint.First());
        }
    }
}