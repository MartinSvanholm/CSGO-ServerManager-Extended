using CSGO_ServerManager_Extended.Data;
using CSGO_ServerManager_Extended.Data.DataAccess;
using CSGOServerInterface.Server.MapPoolNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Map = CSGOServerInterface.Server.MapPoolNS.Map;

namespace CSGO_ServerManager_Extended.Repositories.MapPoolRepository
{
    public interface IMapPoolRepository
    {
        Task DeleteMapPool(MapPool mapPool);
        Task<List<MapPool>> GetAllMapPools(Expression<Func<MapPool, bool>> condition = null);
        Task<MapPool> GetMapPool(Expression<Func<MapPool, bool>> condition = null);
        Task InsertMapPool(MapPool mapPool);
        Task UpdateMapPool(MapPool mapPool);
    }

    public class MapPoolRepository : IMapPoolRepository
    {
        private readonly IDataAccess _dataAccess;

        public MapPoolRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<MapPool> GetMapPool(Expression<Func<MapPool, bool>> condition = null)
        {
            MapPool mapPool = await _dataAccess.GetAsync(condition);

            mapPool.Maps.AddRange(await GetMapsByMapPool(mapPool));

            return mapPool;
        }

        public async Task<List<MapPool>> GetAllMapPools(Expression<Func<MapPool, bool>> condition = null)
        {
            List<MapPool> mapPools = await _dataAccess.GetAllAsync(condition);

            foreach (MapPool mapPool in mapPools)
            {
                mapPool.Maps.AddRange(await GetMapsByMapPool(mapPool));
            }

            return mapPools;
        }

        public async Task InsertMapPool(MapPool mapPool)
        {
            foreach (Map map in mapPool.Maps)
            {
                if(await _dataAccess.CountAsync<Map>(m => m.DisplayName == map.DisplayName) == 0)
                    await _dataAccess.InsertDataAsync(map);

                await _dataAccess.ExecuteAsync(SqlConstants.InsertIntoMapPoolMap, mapPool.Name, map.DisplayName);
            }

            await _dataAccess.InsertDataAsync(mapPool);
        }

        public async Task UpdateMapPool(MapPool mapPool)
        {
            await _dataAccess.ExecuteAsync("delete from MapPoolMap where MapPoolname = ?;", mapPool.Name);

            foreach (Map map in mapPool.Maps)
            {
                if (await _dataAccess.CountAsync<Map>(m => m.DisplayName == map.DisplayName) == 0)
                    await _dataAccess.InsertDataAsync(map);

                await _dataAccess.ExecuteAsync(SqlConstants.InsertIntoMapPoolMap, mapPool.Name, map.DisplayName);
            }

            await DeleteMapsNotUsed();
        }

        public async Task DeleteMapPool(MapPool mapPool)
        {
            await _dataAccess.ExecuteAsync("delete from MapPoolMap where MapPoolname = ?", mapPool.Name);

            await _dataAccess.DeleteDataAsync(mapPool);

            await DeleteMapsNotUsed();
        }

        private async Task<List<Map>> GetMapsByMapPool(MapPool mapPool)
        {
            return await _dataAccess.QueryAsync<Map>(SqlConstants.GetMapsByMapPool, mapPool.Name);
        }

        private async Task DeleteMapsNotUsed()
        {
            List<Map> maps = await _dataAccess.QueryAsync<Map>("select * from Map");

            foreach (Map map in maps)
            {
                if (await _dataAccess.CountAsync<MapPoolMap>(mp => mp.MapDisplayName == map.DisplayName) == 0)
                    await _dataAccess.DeleteDataAsync(map);
            }
        }
    }
}
