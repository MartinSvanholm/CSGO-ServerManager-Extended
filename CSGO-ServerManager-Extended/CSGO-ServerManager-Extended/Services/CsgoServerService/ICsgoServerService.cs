using CsgoServerInterface.CsgoServer;

namespace CSGO_ServerManager_Extended.Services.CsgoServerService
{
    public interface ICsgoServerService
    {
        AbstractCsgoServer SelectedServerforNavigation { get; set; }

        Task<DatHostServer> GetDatHostServer(string id);
        Task<List<DatHostServer>> GetDatHostServers();
    }
}