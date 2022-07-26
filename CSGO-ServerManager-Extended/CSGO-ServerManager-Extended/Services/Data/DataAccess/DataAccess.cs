using CSGO_ServerManager_Extended.Models.DTOs;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CSGO_ServerManager_Extended.Services.DataAccess
{
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

            await CreateTableAsync<CsgoServerDTO>();
        }

        public async Task CreateTableAsync<T>() where T : new()
        {
            await Db.CreateTableAsync<T>();
        }

        public async Task<List<T>> GetListAsync<T>() where T : new()
        {
            return await Db.Table<T>().ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        public async Task<T> GetByConditionAsync<T>(Expression<Func<T, bool>> condition) where T : new()
        {
            return await Db.Table<T>().Where(condition).FirstOrDefaultAsync();
        }

        public async Task InsertDataAsync(object data)
        {
            await Db.InsertAsync(data);
        }

        public async Task UpdateDataAsync(object data)
        {
            var result = await Db.UpdateAsync(data);
        }

        public async Task DeleteDataAsync(object data)
        {
            await Db.DeleteAsync(data);
        }
    }
}
