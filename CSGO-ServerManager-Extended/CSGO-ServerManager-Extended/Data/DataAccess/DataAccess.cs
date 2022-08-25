using CsgoServerInterface.CsgoServer;
using System.Linq.Expressions;
using SQLite;

namespace CSGO_ServerManager_Extended.Data.DataAccess
{
    public interface IDataAccess
    {
        Task DeleteDataAsync(object data);
        Task<List<T>> GetAllAsync<T>() where T : new();
        Task<List<T>> GetByConditionAsync<T>(Expression<Func<T, bool>> condition) where T : new();
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

            Task.Run(async () => await Init());
        }

        public async Task Init()
        {
            if (Db != null)
                return;

            Db = new SQLiteAsyncConnection(_dbPath);

            try
            {
                await Db.CreateTableAsync<CsgoServer>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<T>> GetAllAsync<T>() where T : new()
        {
            return await Db.Table<T>().ToListAsync();
        }

        public async Task<T> GetById<T>(object id) where T : new()
        {
            return await Db.GetAsync<T>(id);
        }

        public async Task<List<T>> GetByConditionAsync<T>(Expression<Func<T, bool>> condition) where T : new()
        {
            return await Db.Table<T>().Where(condition).ToListAsync();
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
