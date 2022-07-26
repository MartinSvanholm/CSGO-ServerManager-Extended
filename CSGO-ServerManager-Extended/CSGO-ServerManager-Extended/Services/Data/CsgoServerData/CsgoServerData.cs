using CSGO_ServerManager_Extended.Models.DTOs;
using CSGO_ServerManager_Extended.Models.Mappers;
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
            var result = await _dataAccess.GetListAsync<CsgoServerDTO>();

            List<ICsgoServer> servers = new();
            foreach (CsgoServerDTO dto in result)
            {
                servers.Add(CsgoServerMapper.MapToCsgoServer(dto));
            }

            return servers;
        }

        public async Task<ICsgoServer> CsgoServers_GetByName(string name)
        {
            var result = await _dataAccess.GetByConditionAsync<CsgoServerDTO>(s => s.Name == name);

            if (result == null)
                throw new ArgumentNullException(nameof(name), $"Cannot find a server with the name of {name}");
            else
                return CsgoServerMapper.MapToCsgoServer(result);
        }

        public async Task CsgoServers_Create(CsgoServer csgoServer)
        {

            CsgoServerDTO dto = CsgoServerMapper.MapToCsgoServerDTO(csgoServer);

            await _dataAccess.InsertDataAsync(dto);
        }

        public async Task CsgoServers_Update(CsgoServer csgoServer)
        {
            CsgoServerDTO dto = CsgoServerMapper.MapToCsgoServerDTO(csgoServer);

            await _dataAccess.UpdateDataAsync(dto);
        }

        public async Task CsgoServer_Delete(CsgoServer csgoServer)
        {
            CsgoServerDTO dto = CsgoServerMapper.MapToCsgoServerDTO(csgoServer);

            await _dataAccess.DeleteDataAsync(dto);
        }
    }
}
