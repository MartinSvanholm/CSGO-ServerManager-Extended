﻿using CoreRCON.Parsers.Standard;
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
using Microsoft.Maui.Controls;
using System.Reflection.Metadata.Ecma335;
using Map = CSGOServerInterface.Server.MapPoolNS.Map;

namespace CSGO_ServerManager_Extended.Services.CsgoServerService;

public interface ICsgoServerService
{
    ICsgoServer Server { get; set; }

    event EventHandler ServerChanged;
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
    Task StartKnife(ICsgoServer server);
    Task StartMatch(ICsgoServer server);
    Task StartMatchWithOvertime(ICsgoServer server);
    Task StartNadePractice(ICsgoServer server);
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

    public event EventHandler ServerChanged;

    public async Task<List<ICsgoServer>> GetCsgoServers()
    {
        List<ICsgoServer> csgoServers = new List<ICsgoServer>();

        try
        {
            csgoServers.AddRange(await _csgoServerRepository.GetCsgoServers());

            foreach (ICsgoServer csgoServer in csgoServers)
            {
                Task task = CheckConnection(csgoServer);
            }

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

            if(_settingsService.DathostAccountIsConnected)
            {
                List<ServerSettings> dathostServerSettings = new List<ServerSettings>(await _serverSettingsRepository.GetServerSettingsByCondition(s => s.IsDathostServer && s.IsFavourite));

                foreach (ServerSettings dathostServerSetting in dathostServerSettings)
                {
                    csgoServers.Add(await GetDatHostServer(dathostServerSetting.CsgoServerId));
                }
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

    public async Task StartMatch(ICsgoServer server)
    {
        try
        {
            if(!string.IsNullOrEmpty(server.ServerSettings.MatchCommand))
                await server.RunCommand(server.ServerSettings.MatchCommand, _httpClient);
            else
                await server.RunCommand(Preferences.Get(GlobalServerCommandsConstants.MatchCommand, CsgoServerConstants.DefaultMatchCommand), _httpClient);
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

    public async Task StartMatchWithOvertime(ICsgoServer server)
    {
        try
        {
            if(!string.IsNullOrEmpty(server.ServerSettings.OvertimeCommand))
                await server.RunCommand(server.ServerSettings.OvertimeCommand, _httpClient);
            else
                await server.RunCommand(Preferences.Get(GlobalServerCommandsConstants.OvertimeCommand, CsgoServerConstants.DefaultOvertimeMatchCommand), _httpClient);
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

    public async Task StartKnife(ICsgoServer server)
    {
        try
        {
            if (!string.IsNullOrEmpty(server.ServerSettings.KnifeCommand))
                await server.RunCommand(server.ServerSettings.KnifeCommand, _httpClient);
            else
                await server.RunCommand(Preferences.Get(GlobalServerCommandsConstants.KnifeCommand, CsgoServerConstants.DefaultKnifeCommand), _httpClient);
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

    public async Task StartNadePractice(ICsgoServer server)
    {
        try
        {
            if(!string.IsNullOrEmpty(server.ServerSettings.PracticeCommand))
                await server.RunCommand(server.ServerSettings.PracticeCommand, _httpClient);
            else
                await server.RunCommand(Preferences.Get(GlobalServerCommandsConstants.PracticeCommand, CsgoServerConstants.DefaultPracticeCommand), _httpClient);
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

    public async Task CheckConnection(ICsgoServer csgoServer)
    {
        try
        {
            Status status = await csgoServer.GetConnection();
            csgoServer.MapBeingPlayed = new Map { DisplayName = status.Map, MapName = status.Map };
            csgoServer.IsOn = true;
            csgoServer.PlayersOnline = status.Humans;

            ServerChanged?.Invoke(this, new());
        }
        catch (Exception)
        {

        }
    }
}