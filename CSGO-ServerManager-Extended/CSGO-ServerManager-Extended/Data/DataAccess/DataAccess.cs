using System.Linq.Expressions;
using SQLite;
using CSGOServerInterface.Server.CsgoServerSettings;
using CSGOServerInterface.Server.CsgoServer;
using CSGOServerInterface.Server.MapPoolNS;
using Map = CSGOServerInterface.Server.MapPoolNS.Map;

namespace CSGO_ServerManager_Extended.Data.DataAccess
{
    public interface IDataAccess
    {
        Task DeleteDataAsync(object data);
        Task<int> ExecuteAsync(string query, params object[] parameters);
        Task<int> CountAsync<T>(Expression<Func<T, bool>> condition = null) where T : new();
        Task<List<T>> GetAllAsync<T>(Expression<Func<T, bool>> condition = null) where T : new();
        Task<T> GetAsync<T>(Expression<Func<T, bool>> condition = null) where T : new();
        Task InitMapPools();
        Task InsertDataAsync(IEnumerable<object> data);
        Task InsertDataAsync(object data);
        Task<List<T>> QueryAsync<T>(string query, params object[] parameters) where T : new();
        Task UpdateDataAsync(object data);
    }

    public class DataAccess : IDataAccess
    {
        private SQLiteAsyncConnection Db { get; set; }
        private readonly string _dbPath;

        public DataAccess(string dbPath)
        {
            _dbPath = dbPath;

            Task init = InitDb();
            init.Wait();
        }

        /// <summary>
        /// This method initilizes the database tables.
        /// </summary>
        /// <returns></returns>
        private async Task InitDb()
        {
            if (Db != null)
                return;

            Db = new SQLiteAsyncConnection(_dbPath);

            Task[] tableTasks = { Db.CreateTableAsync<CsgoServer>(), Db.CreateTableAsync<ServerSettings>(), Db.CreateTableAsync<Map>(), Db.CreateTableAsync<MapPool>(), Db.CreateTableAsync<MapPoolMap>() };

            Task.WaitAll(tableTasks);
        }

        public async Task<List<T>> QueryAsync<T>(string query, params object[] parameters) where T : new()
        {
            return await Db.QueryAsync<T>(query, parameters);
        }

        public async Task<int> ExecuteAsync(string query, params object[] parameters)
        {
            return await Db.ExecuteAsync(query, parameters);
        }

        public async Task<int> CountAsync<T>(Expression<Func<T, bool>> condition = null) where T : new()
        {
            if (condition != null)
                return await Db.Table<T>().CountAsync(condition);
            else
                return await Db.Table<T>().CountAsync();
        }

        public async Task<List<T>> GetAllAsync<T>(Expression<Func<T, bool>> condition = null) where T : new()
        {
            if (condition != null)
                return await Db.Table<T>().Where(condition).ToListAsync();
            else
                return await Db.Table<T>().ToListAsync();
        }

        public async Task<T> GetAsync<T>(Expression<Func<T, bool>> condition = null) where T : new()
        {
            return await Db.Table<T>().Where(condition).FirstOrDefaultAsync();
        }

        public async Task InsertDataAsync(object data)
        {
            await Db.InsertAsync(data);
        }

        public async Task InsertDataAsync(IEnumerable<object> data)
        {
            await Db.InsertAllAsync(data);
        }

        public async Task UpdateDataAsync(object data)
        {
            await Db.UpdateAsync(data);
        }

        public async Task DeleteDataAsync(object data)
        {
            await Db.DeleteAsync(data);
        }

        public async Task InitMapPools()
        {
            if (await Db.Table<MapPool>().CountAsync() == 0)
            {
                List<Map> maps = InitMaps();
                await InsertDataAsync(maps);

                MapPool activeDutyMapPool = new("Active duty", maps);
                await InsertDataAsync(activeDutyMapPool);

                foreach (Map map in activeDutyMapPool.Maps)
                {
                    await ExecuteAsync(SqlConstants.InsertIntoMapPoolMap, activeDutyMapPool.Name, map.DisplayName);
                }
            }
        }

        private List<Map> InitMaps()
        {
            Map mirage = new("Mirage", "de_mirage");
            Map overpass = new("Overpass", "de_overpass");
            Map inferno = new("Inferno", "de_inferno");
            Map nuke = new("Nuke", "de_nuke");
            Map ancient = new("Ancient", "de_ancient");
            Map vertigo = new("Vertigo", "de_vertigo");
            Map dust2 = new("Dust 2", "de_dust2");

            return new List<Map> { mirage, overpass, inferno, nuke, ancient, vertigo, dust2 };
        }
    }
}
