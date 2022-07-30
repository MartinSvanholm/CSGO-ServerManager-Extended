using CsgoServerInterface.CsgoServer;

namespace CSGO_ServerManager_Extended.Services.Data.CsgoServerData
{
    public interface ICsgoServerData
    {
        Task CsgoServers_Create(CsgoServer csgoServer);
        Task<List<ICsgoServer>> CsgoServers_GetAll();
        Task<ICsgoServer> CsgoServers_GetByName(string name);
        Task CsgoServers_Update(CsgoServer csgoServer);
        Task CsgoServer_Delete(CsgoServer csgoServer);
    }
}