using CsgoServerInterface.CsgoServer;
using CSGOServerInterface.Server.DathostServer;

namespace CSGO_ServerManager_Extended.Services.CsgoServerService
{
    public interface ICsgoServerService
    {
        ICsgoServer Server { get; set; }

        Task ChangeMap(string map);
        Task<DatHostCsgoServer> GetDatHostServer(string id);
        Task<List<DatHostCsgoServer>> GetDatHostServers();
        Task PauseUnpauseMatch(bool isMatchPaused);
        Task RunCommand(string command);
        Task StartKnife(string cfg = null);
        Task StartMatch(bool withOvertime, string cfg = null);
        Task StartNadePractice(string cfg = null);
        Task StartStopServer(ICsgoServer server);
    }
}