using CsgoServerInterface.CsgoServer;

namespace CSGO_ServerManager_Extended.Services.CsgoServerService
{
    public interface ICsgoServerService
    {
        AbstractCsgoServer SelectedServerforNavigation { get; set; }

        Task<List<DatHostServer>> GetDatHostServers();
    }
}