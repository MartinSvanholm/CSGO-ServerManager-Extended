using CSGO_ServerManager_Extended.Data.DataAccess;
using CSGO_ServerManager_Extended.Repositories.CsgoServerSettingsRepository;
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
        private readonly IServerSettingsRepository _serverSettingsRepository;

        public CsgoServerRepository(IDataAccess dataAccess, IServerSettingsRepository serverSettingsRepository) : base(dataAccess)
        {
            _serverSettingsRepository = serverSettingsRepository;
        }

        public async Task<List<CsgoServer>> GetCsgoServers()
        {
            List<CsgoServer> servers = await _dataAccess.GetAllAsync<CsgoServer>();

            foreach (CsgoServer server in servers)
            {
                server.ServerSettings = await _serverSettingsRepository.GetServerSettingsByCsgoServerId(server.Id);
            }

            return servers;
        }

        public async Task<CsgoServer> GetCsgoServerById(string id)
        {
            CsgoServer server = await _dataAccess.GetById<CsgoServer>(id);

            server.ServerSettings = await _serverSettingsRepository.GetServerSettingsByCsgoServerId(server.Id);

            return server;
        }

        public async Task<List<CsgoServer>> GetCsgoServerByCondition(Expression<Func<CsgoServer, bool>> condition)
        {
            var data = await _dataAccess.GetAllAsync<CsgoServer>();
            List<CsgoServer> servers = data.AsQueryable().Where(condition).ToList();

            foreach (CsgoServer server in servers)
            {
                server.ServerSettings = await _serverSettingsRepository.GetServerSettingsByCsgoServerId(server.Id);
            }

            return servers;
        }

        public async Task InsertCsgoServer(CsgoServer csgoServer)
        {
            csgoServer.Id = Guid.NewGuid().ToString();
            csgoServer.ServerSettings.CsgoServerId = csgoServer.Id;

            await _dataAccess.InsertDataAsync(csgoServer);
            await _serverSettingsRepository.InsertServerSettings(csgoServer.ServerSettings);
        }

        public async Task UpdateCsgoServer(CsgoServer csgoServer)
        {
            await _dataAccess.UpdateDataAsync(csgoServer);
            await _serverSettingsRepository.UpdateServerSettings(csgoServer.ServerSettings);
        }

        public async Task DeleteCsgoServer(CsgoServer csgoServer)
        {
            await _dataAccess.DeleteDataAsync(csgoServer);
            await _serverSettingsRepository.DeleteServerSettings(csgoServer.ServerSettings);
        }
    }
}
