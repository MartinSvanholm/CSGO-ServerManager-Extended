using CSGO_ServerManager_Extended.Services.DataAccess;
using CsgoServerInterface.CsgoServer;

namespace CSGO_ServerManager_Extended.Services.Data.CsgoServerData
{
    public class CsgoServerData : ICsgoServerData
    {
        private readonly IDataAccess _dataAccess;

        public CsgoServerData(IDataAccess db)
        {
            _dataAccess = db;
        }

        public async Task<List<ICsgoServer>> CsgoServers_GetAll()
        {
            List<CsgoServer> data = await _dataAccess.GetListAsync<CsgoServer>();

            List<ICsgoServer> result = new();
            result.AddRange(data);

            return result;
        }

        public async Task<ICsgoServer> CsgoServers_GetByName(string name)
        {
            var result = await _dataAccess.GetByConditionAsync<CsgoServer>(s => s.Name == name);

            if (result == null)
                throw new ArgumentNullException(nameof(name), $"Cannot find a server with the name of {name}");
            else
                return result;
        }

        public async Task CsgoServers_Create(CsgoServer csgoServer)
        {
            await _dataAccess.InsertDataAsync(csgoServer);
        }

        public async Task CsgoServers_Update(CsgoServer csgoServer)
        {
            await _dataAccess.UpdateDataAsync(csgoServer);
        }

        public async Task CsgoServer_Delete(CsgoServer csgoServer)
        {
            await _dataAccess.DeleteDataAsync(csgoServer);
        }
    }
}
