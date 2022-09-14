using CSGO_ServerManager_Extended.Repositories.CsgoServerSettingsRepository;
using CSGOServerInterface.Server.CsgoServerSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGO_ServerManager_Extended.Services.CsgoServerSettingsService
{
    public interface IServerSettingsService
    {
        Task<ServerSettings> CreateServerSettings(ServerSettings serverSettings);
        Task DeleteServerSettings(ServerSettings serverSettings);
        Task<ServerSettings> GetServerSettingByCsgoServerId(string csgoServerId);
        Task<ServerSettings> UpdateServerSettings(ServerSettings serverSettings);
    }

    public class ServerSettingsService : IServerSettingsService
    {
        private readonly IServerSettingsRepository _settingsRepository;

        public ServerSettingsService(IServerSettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        public async Task<ServerSettings> GetServerSettingByCsgoServerId(string csgoServerId)
        {
            ServerSettings serverSettings;

            try
            {
                serverSettings = await _settingsRepository.GetServerSettingsByCsgoServerId(csgoServerId);
            }
            catch (Exception e)
            {
                throw new Exception($"Something went wrong: {e.Message}");
            }

            if (serverSettings == null)
                throw new Exception("Could not find any custom server settings");

            return serverSettings;
        }

        public async Task<ServerSettings> CreateServerSettings(ServerSettings serverSettings)
        {
            try
            {
                await _settingsRepository.InsertServerSettings(serverSettings);
            }
            catch (Exception e)
            {
                throw new Exception($"Something went wrong: {e.Message}");
            }

            return serverSettings;
        }

        public async Task<ServerSettings> UpdateServerSettings(ServerSettings serverSettings)
        {
            try
            {
                await _settingsRepository.UpdateServerSettings(serverSettings);
            }
            catch (Exception e)
            {
                throw new Exception($"Something went wrong: {e.Message}");
            }

            return serverSettings;
        }

        public async Task DeleteServerSettings(ServerSettings serverSettings)
        {
            try
            {
                await _settingsRepository.DeleteServerSettings(serverSettings);
            }
            catch (Exception e)
            {
                throw new Exception($"Something went wrong: {e.Message}");
            }
        }
    }
}
