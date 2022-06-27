using CsgoServerInterface.CsgoServer;

namespace CSGO_ServerManager_Extended.Services.CsgoServerService
{
    public interface ICsgoServerService
    {
        ICsgoServer SelectedServerforNavigation { get; set; }

        string GetCfg(string fileName);
        Task<DatHostServer> GetDatHostServer(string id);
        Task<List<DatHostServer>> GetDatHostServers();
    }
}