using CSGO_ServerManager_Extended.Models;
using CSGO_ServerManager_Extended.Models.Constants;
using CSGO_ServerManager_Extended.Pages;
using CSGO_ServerManager_Extended.Repositories.CsgoServerRepository;
using CSGO_ServerManager_Extended.Repositories.CsgoServerSettingsRepository;
using CSGO_ServerManager_Extended.Services.CsgoServerSettingsService;
using CSGO_ServerManager_Extended.Services.MapPoolService;
using CSGO_ServerManager_Extended.Services.SettingsService;
using CsgoServerInterface.Exceptions;
using CSGOServerInterface.Mappers;
using CSGOServerInterface.Server.CsgoServer;
using CSGOServerInterface.Server.CsgoServerSettings;
using CSGOServerInterface.Server.DathostServer;
using CSGOServerInterface.Server.DTO;
using CSGOServerInterface.Server.MapPoolNS;
using Map = CSGOServerInterface.Server.MapPoolNS.Map;

namespace CSGO_ServerManager_Extended.Services.CsgoServerService;

public interface ICsgoServerService
{
    ICsgoServer Server { get; set; }

    Task<CsgoServer> AddCsgoServer(CsgoServer csgoServer);
    Task ChangeMap(Map map);
    Task<CsgoServer> DeleteCsgoServer(CsgoServer csgoServer);
    Task<ICsgoServer> GetCsgoServerById(string csgoServerId);
    Task<List<ICsgoServer>> GetCsgoServers();
    Task<DatHostCsgoServer> GetDatHostServer(string id, bool updateServerSettings = true, ServerSettings oldServerSettings = null, MapPool oldMapPool = null);
    Task<List<DatHostCsgoServer>> GetDatHostServers();
    Task<List<ICsgoServer>> GetFavouriteCsgoServers();
    Task PauseUnpauseMatch(bool isMatchPaused);
    Task RunCommand(string command);
    Task StartKnife(string cfg = null);
    Task StartMatch(bool withOvertime, string cfg = null);
    Task StartNadePractice(string cfg = null);
    Task StartStopServer(ICsgoServer server);
    Task<CsgoServer> UpdateCsgoServer(CsgoServer csgoServer);
}

public class CsgoServerService : ICsgoServerService
{
    private HttpClient _httpClient;
    private readonly ICsgoServerRepository _csgoServerRepository;
    private readonly ISettingsService _settingsService;
    private readonly IServerSettingsRepository _serverSettingsRepository;
    private readonly IMapPoolService _mapPoolService;
    public ICsgoServer Server { get; set; }

    public CsgoServerService(HttpClient httpClient, ICsgoServerRepository csgoServerRepository, ISettingsService settingsService, IServerSettingsRepository serverSettingsRepository, IMapPoolService mapPoolService)
    {
        _httpClient = httpClient;
        _csgoServerRepository = csgoServerRepository;
        _settingsService = settingsService;
        _serverSettingsRepository = serverSettingsRepository;
        _mapPoolService = mapPoolService;
    }

    public async Task<List<ICsgoServer>> GetCsgoServers()
    {
        List<ICsgoServer> csgoServers = new List<ICsgoServer>();

        try
        {
            csgoServers.AddRange(await _csgoServerRepository.GetCsgoServers());

            if (_settingsService.DathostAccountIsConnected)
                csgoServers.AddRange(await GetDatHostServers());
        }
        catch (Exception e)
        {
            throw new Exception($"Something went wrong: {e.Message}");
        }

        if (csgoServers == null)
            throw new Exception("Could not find any csgo servers, please add a server");

        return csgoServers;
    }

    public async Task<ICsgoServer> GetCsgoServerById(string csgoServerId)
    {
        ICsgoServer csgoServer;

        try
        {
            csgoServer = await _csgoServerRepository.GetCsgoServerById(csgoServerId);
        }
        catch (Exception e)
        {
            throw new Exception($"Something went wrong: {e.Message}");
        }

        if (csgoServer == null)
            throw new Exception("Could not find csgo server");

        return csgoServer;
    }

    public async Task<List<ICsgoServer>> GetFavouriteCsgoServers()
    {
        List<ICsgoServer> csgoServers = new();

        try
        {
            csgoServers.AddRange(await _csgoServerRepository.GetCsgoServerByCondition(s => s.ServerSettings.IsFavourite));

            List<ServerSettings> dathostServerSettings = new List<ServerSettings>(await _serverSettingsRepository.GetServerSettingsByCondition(s => s.IsDathostServer && s.IsFavourite));

            foreach (ServerSettings dathostServerSetting in dathostServerSettings)
            {
                csgoServers.Add(await GetDatHostServer(dathostServerSetting.CsgoServerId));
            }
        }
        catch (Exception e)
        {
            throw new Exception($"Something went wrong: {e.Message}");
        }

        if (csgoServers.Count() == 0)
            throw new Exception("Could not find any csgo servers marked as favourite");

        return csgoServers;
    }

    public async Task<CsgoServer> AddCsgoServer(CsgoServer csgoServer)
    {
        try
        {
            await _csgoServerRepository.InsertCsgoServer(csgoServer);
        }
        catch (Exception e)
        {
            throw new Exception($"Something went wrong: {e.Message}");
        }

        return csgoServer;
    }

    public async Task<CsgoServer> UpdateCsgoServer(CsgoServer csgoServer)
    {
        try
        {
            await _csgoServerRepository.UpdateCsgoServer(csgoServer);
        }
        catch (Exception e)
        {
            throw new Exception($"Something went wrong: {e.Message}");
        }

        return csgoServer;
    }

    public async Task<CsgoServer> DeleteCsgoServer(CsgoServer csgoServer)
    {
        try
        {
            await _csgoServerRepository.DeleteCsgoServer(csgoServer);
        }
        catch (Exception e)
        {
            throw new Exception($"Something went wrong: {e.Message}");
        }

        return csgoServer;
    }

    public async Task<List<DatHostCsgoServer>> GetDatHostServers()
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            throw new CsgoServerException("Device not connected to the internet", System.Net.HttpStatusCode.InternalServerError);

        string uri = _httpClient.BaseAddress.ToString() + "/api/0.1/game-servers";

        using HttpResponseMessage responseMessage = await _httpClient.GetAsync(uri);

        if (responseMessage.IsSuccessStatusCode)
        {
            List<DatHostServerDTO> datHostDTOs = await responseMessage.Content.ReadAsAsync<List<DatHostServerDTO>>();

            for (int i = 0; i < datHostDTOs.Count; i++)
            {
                if (datHostDTOs[i].CsgoSettings == null)
                    datHostDTOs.RemoveAt(i);
            }

            List<DatHostCsgoServer> datHostCsgoServers = DatHostCsgoServerMapper.MapList(datHostDTOs);

            foreach (DatHostCsgoServer datHostCsgoServer in datHostCsgoServers)
            {
                datHostCsgoServer.ServerSettings = await _serverSettingsRepository.GetServerSettingsByCsgoServerId(datHostCsgoServer.Id);

                if (datHostCsgoServer.ServerSettings == null)
                {
                    datHostCsgoServer.ServerSettings = new();
                    datHostCsgoServer.ServerSettings.CsgoServerId = datHostCsgoServer.Id;
                    datHostCsgoServer.ServerSettings.IsDathostServer = true;

                    await _serverSettingsRepository.InsertServerSettings(datHostCsgoServer.ServerSettings);
                }

                if (datHostCsgoServer.ServerSettings.MapPoolName != null)
                    datHostCsgoServer.MapPool = await _mapPoolService.GetMapPoolByName(datHostCsgoServer.ServerSettings.MapPoolName);

                datHostCsgoServer.MapPool ??= new();
            }

            return datHostCsgoServers;
        }
        else
        {
            throw new CsgoServerException(responseMessage.ReasonPhrase, responseMessage.StatusCode);
        }
    }

    public async Task<DatHostCsgoServer> GetDatHostServer(string id, bool updateServerSettings = true, ServerSettings oldServerSettings = null, MapPool oldMapPool = null)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            throw new CsgoServerException("Device not connected to the internet", System.Net.HttpStatusCode.InternalServerError);

        string uri = _httpClient.BaseAddress.ToString() + $"/api/0.1/game-servers/{id}";

        using HttpResponseMessage responseMessage = await _httpClient.GetAsync(uri);

        if (responseMessage.IsSuccessStatusCode)
        {
            var result = await responseMessage.Content.ReadAsAsync<DatHostServerDTO>();
            DatHostCsgoServer datHostCsgoServer = DatHostCsgoServerMapper.MapFromDTO(result);

            if (updateServerSettings)
            {
                datHostCsgoServer.ServerSettings = await _serverSettingsRepository.GetServerSettingsByCsgoServerId(datHostCsgoServer.Id);

                if (datHostCsgoServer.ServerSettings == null)
                {
                    datHostCsgoServer.ServerSettings = new();
                    datHostCsgoServer.ServerSettings.CsgoServerId = datHostCsgoServer.Id;

                    await _serverSettingsRepository.InsertServerSettings(datHostCsgoServer.ServerSettings);
                }

                if (datHostCsgoServer.ServerSettings.MapPoolName != null)
                    datHostCsgoServer.MapPool = await _mapPoolService.GetMapPoolByName(datHostCsgoServer.ServerSettings.MapPoolName);

                datHostCsgoServer.MapPool ??= new();
            }
            else
            {
                datHostCsgoServer.ServerSettings = oldServerSettings;
                datHostCsgoServer.MapPool = oldMapPool;
            }

            return datHostCsgoServer;
        }
        else
        {
            throw new CsgoServerException(responseMessage.ReasonPhrase, responseMessage.StatusCode);
        }
    }

    public async Task StartStopServer(ICsgoServer server)
    {
        try
        {
            if (!server.IsOn)
            {
                await Server.StartServer(_httpClient);
                server.Booting = true;
            }
            else
            {
                await Server.StopServer(_httpClient);
                server.Booting = false;
                server.IsOn = false;
            }
        }
        catch (CsgoServerException serverExeption)
        {
            throw new Exception(server.IsOn ? $"Could not stop server: {serverExeption.Message}" : $"Could not start server: {serverExeption.Message}");
        }
        catch (Exception e)
        {
            throw new Exception($"Something went wrong: {e.Message}");
        }
    }

    public async Task ChangeMap(Map map)
    {
        try
        {
            await Server.RunCommand($"map {map.MapName}", _httpClient);
            Server.MapBeingPlayed = map;
        }
        catch (CsgoServerException serverExeption)
        {
            throw new Exception($"Could not change map: {serverExeption.Message}");
        }
        catch (Exception e)
        {
            throw new Exception($"Something went wrong: {e.Message}");
        }
    }

    public async Task RunCommand(string command)
    {
        try
        {
            await Server.RunCommand(command, _httpClient);
        }
        catch (CsgoServerException serverExeption)
        {
            throw new Exception($"Could not run command: {serverExeption.Message}");
        }
        catch (Exception e)
        {
            throw new Exception($"Something went wrong: {e.Message}");
        }
    }

    public async Task StartMatch(bool withOvertime, string cfg = null)
    {
        try
        {
            if (cfg != null)
            {
                await Server.RunCommand(cfg, _httpClient);
            }
            else
            {
                if (withOvertime)
                    cfg = Preferences.Get(GlobalServerCommandsConstants.OvertimeCommand, null);
                else
                    cfg = Preferences.Get(GlobalServerCommandsConstants.MatchCommand, null);


                if (cfg != null)
                    await Server.RunCommand(cfg, _httpClient);
                else
                {
                    if (!withOvertime)
                        await Server.RunCommand("exec esportliga_start.cfg", _httpClient);
                    else
                        await Server.RunCommand("exec esportliga_start_med_overtime.cfg", _httpClient);
                }
            }
        }
        catch (CsgoServerException serverExeption)
        {
            throw new Exception($"Could not start match: {serverExeption.Message}");
        }
        catch (Exception e)
        {
            throw new Exception($"Something went wrong: {e.Message}");
        }
    }

    public async Task PauseUnpauseMatch(bool isMatchPaused)
    {
        try
        {
            if (!isMatchPaused)
                await Server.RunCommand("mp_pause_match", _httpClient);
            else
                await Server.RunCommand("mp_unpause_match", _httpClient);

        }
        catch (CsgoServerException serverExeption)
        {
            throw new Exception($"Could not pause/unpause match: {serverExeption.Message}");
        }
        catch (Exception e)
        {
            throw new Exception($"Something went wrong: {e.Message}");
        }
    }

    public async Task StartKnife(string cfg = null)
    {
        try
        {
            if (cfg != null)
            {
                await Server.RunCommand(cfg, _httpClient);
            }
            else
            {
                cfg = Preferences.Get(GlobalServerCommandsConstants.KnifeCommand, null);

                if (cfg != null)
                    await Server.RunCommand(cfg, _httpClient);
                else
                    await Server.RunCommand("exec knife.cfg", _httpClient);
            }
        }
        catch (CsgoServerException serverExeption)
        {
            throw new Exception($"Could not start knife round: {serverExeption.Message}");
        }
        catch (Exception e)
        {
            throw new Exception($"Something went wrong: {e.Message}");
        }
    }

    public async Task StartNadePractice(string cfg = null)
    {
        try
        {
            if (cfg != null)
            {
                await Server.RunCommand(cfg, _httpClient);
            }
            else
            {
                cfg = Preferences.Get(GlobalServerCommandsConstants.PracticeCommand, null);

                if (cfg != null)
                    await Server.RunCommand(cfg, _httpClient);
                else
                    await Server.RunCommand("exec train.cfg", _httpClient);
            }
        }
        catch (CsgoServerException serverExeption)
        {
            throw new Exception($"Could not start nade practice: {serverExeption.Message}");
        }
        catch (Exception e)
        {
            throw new Exception($"Something went wrong: {e.Message}");
        }
    }
}