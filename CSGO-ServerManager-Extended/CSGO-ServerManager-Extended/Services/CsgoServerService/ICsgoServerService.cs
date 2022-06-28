using CsgoServerInterface.CsgoServer;

namespace CSGO_ServerManager_Extended.Services.CsgoServerService
{
    public interface ICsgoServerService
    {
        ICsgoServer Server { get; set; }

        Task<ICsgoServer> ChangeMap(string map);
        string GetCfg(string fileName);
        Task<DatHostServer> GetDatHostServer(string id);
        Task<List<DatHostServer>> GetDatHostServers();
        Task<ICsgoServer> PauseUnpauseMatch(bool isMatchPaused);
        Task<ICsgoServer> RunCommand(string command);
        Task<ICsgoServer> StartKnife(string cfg = null);
        Task<ICsgoServer> StartMatch(bool withOvertime, string cfg = null);
        Task<ICsgoServer> StartNadePractice(string cfg = null);
        Task<ICsgoServer> StartStopServer();
    }
}