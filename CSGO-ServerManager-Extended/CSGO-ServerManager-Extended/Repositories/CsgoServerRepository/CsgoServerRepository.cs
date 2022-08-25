using CSGO_ServerManager_Extended.Data.DataAccess;
using CsgoServerInterface.CsgoServer;
using System.Linq.Expressions;

namespace CSGO_ServerManager_Extended.Repositories.CsgoServerRepository
{
    public interface ICsgoServerRepository
    {
        Task<ICsgoServer> GetCsgoServerById(string id);
        Task<List<ICsgoServer>> GetCsgoServers();
        Task InsertCsgoServer(ICsgoServer csgoServer);
        Task UpdateCsgoServer(ICsgoServer csgoServer);
        Task<List<ICsgoServer>> GetCsgoServerByCondition(Expression<Func<CsgoServer, bool>> condition);
        Task DeleteCsgoServer(ICsgoServer csgoServer);
    }

    public class CsgoServerRepository : RepositoryBase, ICsgoServerRepository
    {
        public CsgoServerRepository(IDataAccess dataAccess) : base(dataAccess)
        {
        }

        public async Task<List<ICsgoServer>> GetCsgoServers()
        {
            try
            {
                return new List<ICsgoServer>(await _dataAccess.GetAllAsync<CsgoServer>());
            }
            catch (Exception e)
            {
                throw new Exception($"Something went wrong: {e.Message}");
            }
        }

        public async Task<ICsgoServer> GetCsgoServerById(string id)
        {
            try
            {
                return await _dataAccess.GetById<CsgoServer>(id);
            }
            catch (Exception e)
            {
                throw new Exception($"Something went wrong: {e.Message}");
            }
        }

        public async Task<List<ICsgoServer>> GetCsgoServerByCondition(Expression<Func<CsgoServer, bool>> condition)
        {
            try
            {
                return new List<ICsgoServer>(await _dataAccess.GetByConditionAsync(condition));
            }
            catch (Exception e)
            {
                throw new Exception($"Something went wrong: {e.Message}");
            }
        }

        public async Task InsertCsgoServer(ICsgoServer csgoServer)
        {
            try
            {
                csgoServer.Id = Guid.NewGuid().ToString();

                await _dataAccess.InsertDataAsync(csgoServer);
            }
            catch (Exception e)
            {
                throw new Exception($"Something went wrong: {e.Message}");
            }
        }

        public async Task UpdateCsgoServer(ICsgoServer csgoServer)
        {
            try
            {
                await _dataAccess.UpdateDataAsync(csgoServer);
            }
            catch (Exception e)
            {
                throw new Exception($"Something went wrong: {e.Message}");
            }
        }

        public async Task DeleteCsgoServer(ICsgoServer csgoServer)
        {
            try
            {
                await _dataAccess.DeleteDataAsync(csgoServer);
            }
            catch (Exception e)
            {
                throw new Exception($"Something went wrong: {e.Message}");
            }
        }
    }
}
