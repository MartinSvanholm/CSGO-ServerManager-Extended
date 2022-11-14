using CSGO_ServerManager_Extended.Repositories.MapPoolRepository;
using CSGOServerInterface.Server.MapPoolNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Map = CSGOServerInterface.Server.MapPoolNS.Map;

namespace CSGO_ServerManager_Extended.Services.MapPoolService;

public interface IMapPoolService
{
    Task DeleteMapPool(MapPool mapPool);
    Task<List<MapPool>> GetAllMapPools();
    Task<MapPool> GetMapPoolByName(string name);
    Task InsertMapPool(MapPool mapPool);
    Task UpdateMapPool(MapPool mapPool);
}

public class MapPoolService : IMapPoolService
{
    private readonly IMapPoolRepository _mapPoolRepository;

    public MapPoolService(IMapPoolRepository mapPoolRepository)
    {
        _mapPoolRepository = mapPoolRepository;
    }

    public async Task<List<MapPool>> GetAllMapPools()
    {
        try
        {
            return await _mapPoolRepository.GetAllMapPools();
        }
        catch (Exception e)
        {
            throw new Exception($"Could not get map pools: {e.Message}");
        }
    }

    public async Task<MapPool> GetMapPoolByName(string name)
    {
        try
        {
            return await _mapPoolRepository.GetMapPool(mp => mp.Name == name);
        }
        catch (Exception e)
        {
            throw new Exception($"Could not get map pool: {e.Message}");
        }
    }

    public async Task InsertMapPool(MapPool mapPool)
    {
        try
        {
            await _mapPoolRepository.InsertMapPool(mapPool);
        }
        catch (Exception e)
        {
            throw new Exception($"Could not create map pool: {e.Message}");
        }
    }

    public async Task UpdateMapPool(MapPool mapPool)
    {
        try
        {
            await _mapPoolRepository.UpdateMapPool(mapPool);
        }
        catch (Exception e)
        {
            throw new Exception($"Could not update map pool: {e.Message}");
        }
    }

    public async Task DeleteMapPool(MapPool mapPool)
    {
        try
        {
            await _mapPoolRepository.DeleteMapPool(mapPool);
        }
        catch (Exception e)
        {
            throw new Exception($"Could not delete map pool: {e.Message}");
        }
    }
}
