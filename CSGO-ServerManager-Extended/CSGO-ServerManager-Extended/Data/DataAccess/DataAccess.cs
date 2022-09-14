using CsgoServerInterface.CsgoServer;
using System.Linq.Expressions;
using SQLite;
using CSGOServerInterface.Server.CsgoServerSettings;

namespace CSGO_ServerManager_Extended.Data.DataAccess
{
    public interface IDataAccess
    {
        Task DeleteDataAsync(object data);
        Task<List<T>> GetAllAsync<T>() where T : new();
        Task<T> GetById<T>(object id) where T : new();
        Task Init();
        Task InsertDataAsync(object data);
        Task UpdateDataAsync(object data);
    }

    public class DataAccess : IDataAccess
    {
        private SQLiteAsyncConnection Db { get; set; }
        private readonly string _dbPath;

        public DataAccess(string dbPath)
        {
            _dbPath = dbPath;

            var init = Init();

            Task.WaitAll(init);
        }

        public async Task Init()
        {
            if (Db != null)
                return;

            Db = new SQLiteAsyncConnection(_dbPath);

            Task[] tasks = { Db.CreateTableAsync<CsgoServer>(), Db.CreateTableAsync<ServerSettings>()};

            Task.WaitAll(tasks);
        }

        public async Task<List<T>> GetAllAsync<T>() where T : new()
        {
            return await Db.Table<T>().ToListAsync();
        }

        public async Task<T> GetById<T>(object id) where T : new()
        {
            return await Db.GetAsync<T>(id);
        }

        public async Task InsertDataAsync(object data)
        {
            await Db.InsertAsync(data);
        }

        public async Task UpdateDataAsync(object data)
        {
            await Db.UpdateAsync(data);
        }

        public async Task DeleteDataAsync(object data)
        {
            await Db.DeleteAsync(data);
        }
    }
}
