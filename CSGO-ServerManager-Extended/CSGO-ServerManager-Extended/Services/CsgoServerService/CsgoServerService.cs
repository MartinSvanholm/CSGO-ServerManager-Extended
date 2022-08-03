using CSGO_ServerManager_Extended.Services.Data.CsgoServerData;
using CsgoServerInterface.CsgoServer;
using CsgoServerInterface.Exceptions;
using CSGOServerInterface.Mappers;
using CSGOServerInterface.Server.DathostServer;
using CSGOServerInterface.Server.DTO;
using MudBlazor;

namespace CSGO_ServerManager_Extended.Services.CsgoServerService;

public class CsgoServerService : ICsgoServerService
{
    public CsgoServerService(HttpClient httpClient, ICsgoServerData csgoServerData)
    {
        _httpClient = httpClient;
        _csgoServerData = csgoServerData;
    }

    private HttpClient _httpClient;
    private readonly ICsgoServerData _csgoServerData;

    public ICsgoServer Server { get; set; }

    public async Task<List<DatHostCsgoServer>> GetDatHostServers()
    {
        if (!(Connectivity.Current.NetworkAccess == NetworkAccess.Internet))
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

            return DatHostCsgoServerMapper.MapList(datHostDTOs);
        }
        else
        {
            throw new CsgoServerException(responseMessage.ReasonPhrase, responseMessage.StatusCode);
        }
    }

    public async Task<DatHostCsgoServer> GetDatHostServer(string id)
    {
        if (!(Connectivity.Current.NetworkAccess == NetworkAccess.Internet))
            throw new CsgoServerException("Device not connected to the internet", System.Net.HttpStatusCode.InternalServerError);

        string uri = _httpClient.BaseAddress.ToString() + $"/api/0.1/game-servers/{id}";

        using HttpResponseMessage responseMessage = await _httpClient.GetAsync(uri);

        if (responseMessage.IsSuccessStatusCode)
        {
            var result = await responseMessage.Content.ReadAsAsync<DatHostServerDTO>();
            return DatHostCsgoServerMapper.Map(result);
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

    public async Task ChangeMap(string map)
    {
        try
        {
            await Server.RunCommand($"map de_{map.ToLower()}", _httpClient);
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
            if (cfg == null)
            {
                if (!withOvertime)
                    await Server.RunCommand("exec esportliga_start.cfg", _httpClient);
                else
                    await Server.RunCommand("exec esportliga_start_med_overtime.cfg", _httpClient);
            }
            else
            {
                await Server.RunCommand(cfg, _httpClient);
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
            if (cfg == null)
                await Server.RunCommand("exec knife.cfg", _httpClient);
            else
                await Server.RunCommand(cfg, _httpClient);
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
            if (cfg == null)
                await Server.RunCommand("exec train.cfg", _httpClient);
            else
                await Server.RunCommand(cfg, _httpClient);
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