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
            await UpdateDbEntityAsync(environmentInfosDTO);
            return environmentInfosDTO;
        }

        public async Task<DbEntity> GetDbEntityAsync(string Id)
        {
            var data = await _database.StringGetAsync(Id);

            if (data.IsNullOrEmpty)
            {
                return null;
            }

            return JsonSerializer.Deserialize<DbEntity>(data, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<DbEntity> UpdateDbEntityAsync(DbEntity DbEntity)
        {
            // Scriviamo in una chiave condivisa, una lista di cose in cui andiamo ad aggiungere
            // O se possibile andiamo non sulla stessa chiave ma solo su un qualcosa di comune con cui riprendere tutti i valori che poi ordineremo
            // Collection dove l'id dell'oggetto è l'hostname e il valore è l'oggetto DbEntity
            // Random wait da 100 a 300 ms, La popoliamo, riprendiamo tutti quelli della collection, e li ritorniamo in ordine
            var created = await _database.StringSetAsync(DbEntity.BuyerId, JsonSerializer.Serialize(DbEntity));

            if (!created)
            {
                _logger.LogInformation("Problem occur persisting the item.");
                return null;
            }

            _logger.LogInformation("DbEntity item persisted succesfully.");

            return await GetDbEntityAsync(DbEntity.BuyerId);
        }

        private IServer GetServer()
        {
            var endpoint = _redis.GetEndPoints();
            return _redis.GetServer(endpoint.First());
        }
    }
}