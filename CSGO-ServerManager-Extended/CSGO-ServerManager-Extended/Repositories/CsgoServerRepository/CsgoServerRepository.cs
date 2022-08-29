using CSGO_ServerManager_Extended.Data.DataAccess;
using CsgoServerInterface.CsgoServer;
using System.Linq;
using System.Linq.Expressions;

namespace CSGO_ServerManager_Extended.Repositories.CsgoServerRepository
{
    public interface ICsgoServerRepository
    {
        Task<CsgoServer> GetCsgoServerById(string id);
        Task<List<CsgoServer>> GetCsgoServers();
        Task InsertCsgoServer(CsgoServer csgoServer);
        Task UpdateCsgoServer(CsgoServer csgoServer);
        Task<List<CsgoServer>> GetCsgoServerByCondition(Expression<Func<CsgoServer, bool>> condition);
        Task DeleteCsgoServer(CsgoServer csgoServer);
    }

    public class CsgoServerRepository : RepositoryBase, ICsgoServerRepository
    {
        public CsgoServerRepository(IDataAccess dataAccess) : base(dataAccess)
        {
        }

        public async Task<List<CsgoServer>> GetCsgoServers()
        {
            try
            {
                return await _dataAccess.GetAllAsync<CsgoServer>();
            }
            catch (Exception e)
            {
                throw new Exception($"Something went wrong: {e.Message}");
            }
        }

        public async Task<CsgoServer> GetCsgoServerById(string id)
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

        public async Task<List<CsgoServer>> GetCsgoServerByCondition(Expression<Func<CsgoServer, bool>> condition)
        {
            try
            {
                var data = await _dataAccess.GetAllAsync<CsgoServer>();
                return data.AsQueryable().Where(condition).ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Something went wrong: {e.Message}");
            }
        }

        public async Task InsertCsgoServer(CsgoServer csgoServer)
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

        public async Task UpdateCsgoServer(CsgoServer csgoServer)
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

        public async Task DeleteCsgoServer(CsgoServer csgoServer)
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
