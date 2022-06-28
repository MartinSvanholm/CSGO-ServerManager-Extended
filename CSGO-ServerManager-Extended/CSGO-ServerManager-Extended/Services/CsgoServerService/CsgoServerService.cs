using CsgoServerInterface.CsgoServer;

namespace CSGO_ServerManager_Extended.Services.CsgoServerService;

public class CsgoServerService : ICsgoServerService
{
    public CsgoServerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private HttpClient _httpClient;

    public ICsgoServer Server { get; set; }
    private bool _isMatchPaused { get; set; }

    public async Task<List<DatHostServer>> GetDatHostServers()
    {
        string uri = _httpClient.BaseAddress.ToString() + "/api/0.1/game-servers";

        using HttpResponseMessage responseMessage = await _httpClient.GetAsync(uri);

        if (responseMessage.IsSuccessStatusCode)
        {
            List<DatHostServer> datHosts = await responseMessage.Content.ReadAsAsync<List<DatHostServer>>();

            for (int i = 0; i < datHosts.Count; i++)
            {
                if (datHosts[i].CsgoSettings == null)
                    datHosts.RemoveAt(i);
            }

            return datHosts;
        }
        else
        {
            throw new HttpRequestException(responseMessage.ReasonPhrase, null, responseMessage.StatusCode);
        }
    }

    public async Task<DatHostServer> GetDatHostServer(string id)
    {
        string uri = _httpClient.BaseAddress.ToString() + $"/api/0.1/game-servers/{id}";

        using HttpResponseMessage responseMessage = await _httpClient.GetAsync(uri);

        if (responseMessage.IsSuccessStatusCode)
        {
            return await responseMessage.Content.ReadAsAsync<DatHostServer>();
        }
        else
        {
            throw new HttpRequestException(responseMessage.ReasonPhrase, null, responseMessage.StatusCode);
        }
    }

    public async Task<ICsgoServer> StartStopServer()
    {
        try
        {
            if (!Server.On)
            {
                Server = await Server.StartServer(_httpClient);
                Server.Booting = true;
                return Server;
            }
            else
            {
                Server = await Server.StopServer(_httpClient);
                Server.On = false;
                Server.Booting = false;
                return Server;
            }
        }
        catch (Exception)
        {
            throw;
        }

    }

    public async Task<ICsgoServer> ChangeMap(string map)
    {
        try
        {
            return await Server.RunCommand(_httpClient, $"map de_{map.ToLower()}");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ICsgoServer> RunCommand(string command)
    {
        try
        {
            return await Server.RunCommand(_httpClient, command);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ICsgoServer> StartMatch(bool withOvertime, string cfg = null)
    {
        try
        {
            if (cfg == null)
            {
                if (!withOvertime)
                    return await Server.RunCommand(_httpClient, GetCfg("esportliga_start.txt"));
                else
                    return await Server.RunCommand(_httpClient, GetCfg("esportliga_start_med_overtime.txt"));
            }
            else
            {
                return await Server.RunCommand(_httpClient, cfg);
            }
        }
        catch (Exception)
        {
            throw;
        }

    }

    public async Task<ICsgoServer> PauseUnpauseMatch(bool isMatchPaused)
    {
        try
        {
            if (!isMatchPaused)
                return await Server.RunCommand(_httpClient, "mp_pause_match");
            else
                return await Server.RunCommand(_httpClient, "mp_unpause_match");

        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ICsgoServer> StartKnife(string cfg = null)
    {
        try
        {
            if (cfg == null)
                return await Server.RunCommand(_httpClient, GetCfg("knife.txt"));
            else
                return await Server.RunCommand(_httpClient, cfg);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ICsgoServer> StartNadePractice(string cfg = null)
    {
        try
        {
            if (cfg == null)
                return await Server.RunCommand(_httpClient, GetCfg("pracc.txt"));
            else
                return await Server.RunCommand(_httpClient, cfg);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public string GetCfg(string fileName)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\Cfg\", fileName);

        string[] commands = File.ReadAllLines(path);

        for (int i = 0; i < commands.Length; i++)
        {
            commands[i] = commands[i].Trim();
            commands[i].StartsWith("//");
        }

        return string.Join("; ", commands);
    }
}