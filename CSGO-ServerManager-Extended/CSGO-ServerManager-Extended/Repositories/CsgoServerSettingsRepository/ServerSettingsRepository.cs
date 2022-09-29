using CSGO_ServerManager_Extended.Data.DataAccess;
using CSGOServerInterface.Exceptions;
using CSGOServerInterface.Server.CsgoServerSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CSGO_ServerManager_Extended.Repositories.CsgoServerSettingsRepository
{
    public interface IServerSettingsRepository
    {
        Task DeleteServerSettings(ServerSettings serverSettings);
        Task<List<ServerSettings>> GetServerSettings();
        Task<List<ServerSettings>> GetServerSettingsByCondition(Expression<Func<ServerSettings, bool>> condition);
        Task<ServerSettings> GetServerSettingsByCsgoServerId(string csgoServerId);
        Task<ServerSettings> GetServerSettingsById(int id);
        Task InsertServerSettings(ServerSettings serverSettings);
        Task UpdateServerSettings(ServerSettings serverSettings);
    }

    public class ServerSettingsRepository : RepositoryBase, IServerSettingsRepository
    {
        public ServerSettingsRepository(IDataAccess dataAccess) : base(dataAccess)
        {

        }

        public async Task<List<ServerSettings>> GetServerSettings()
        {
            return await _dataAccess.GetAllAsync<ServerSettings>();
        }

        public async Task<ServerSettings> GetServerSettingsById(int id)
        {
            return await _dataAccess.GetById<ServerSettings>(id);
        }

        public async Task<ServerSettings> GetServerSettingsByCsgoServerId(string csgoServerId)
        {
            var data = await _dataAccess.GetAllAsync<ServerSettings>();
            return data.AsQueryable().Where(s => s.CsgoServerId == csgoServerId).FirstOrDefault();
        }

        public async Task<List<ServerSettings>> GetServerSettingsByCondition(Expression<Func<ServerSettings, bool>> condition)
        {
            var data = await _dataAccess.GetAllAsync<ServerSettings>();
            return data.AsQueryable().Where(condition).ToList();
        }

        public async Task InsertServerSettings(ServerSettings serverSettings)
        {
            await _dataAccess.InsertDataAsync(serverSettings);
        }

        public async Task UpdateServerSettings(ServerSettings serverSettings)
        {
            await _dataAccess.UpdateDataAsync(serverSettings);
        }

        public async Task DeleteServerSettings(ServerSettings serverSettings)
        {
            await _dataAccess.DeleteDataAsync(serverSettings);
        }
    }
}
