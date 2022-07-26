using System.Linq.Expressions;

namespace CSGO_ServerManager_Extended.Services.DataAccess
{
    public interface IDataAccess
    {
        Task CreateTableAsync<T>() where T : new();
        Task DeleteDataAsync(object data);
        Task<T> GetByConditionAsync<T>(Expression<Func<T, bool>> condition) where T : new();
        Task<List<T>> GetListAsync<T>() where T : new();
        Task Init();
        Task InsertDataAsync(object data);
        Task UpdateDataAsync(object data);
    }
}