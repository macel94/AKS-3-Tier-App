using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;

namespace AKS.Three.Tier.App.API.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class InfosController(ILogger<InfosController> logger, ConnectionMultiplexer redis) : ControllerBase
{
    private readonly ILogger<InfosController> _logger = logger;
    private readonly IDatabase _database = redis.GetDatabase();

    [HttpGet]
    public async Task<APIEnvironmentInfosDTO> GetEnvironmentInfos()
    {
        APIEnvironmentInfosDTO environmentInfosDTO = new();
        await environmentInfosDTO.GetIpInfosAsync();
        var currentResults = await GetUpdatedDbEntitiesAsync(environmentInfosDTO);
        environmentInfosDTO.DbEntities = currentResults;
        return environmentInfosDTO;
    }

    private async Task<List<DbEntity?>?> GetUpdatedDbEntitiesAsync(APIEnvironmentInfosDTO currentInfos)
    {
        await Task.Delay(Random.Shared.Next(10, 300));
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

        return currentSet.Select(x => JsonSerializer.Deserialize<DbEntity>(x!)).ToList();
    }
}
