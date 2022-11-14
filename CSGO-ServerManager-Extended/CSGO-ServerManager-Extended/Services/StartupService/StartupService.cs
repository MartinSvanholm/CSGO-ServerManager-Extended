using CSGO_ServerManager_Extended.Data.DataAccess;
using CSGOServerInterface.Server.MapPoolNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Map = CSGOServerInterface.Server.MapPoolNS.Map;

namespace CSGO_ServerManager_Extended.Services.StartupService;

public interface IStartupService
{
    Task InitData();
}

public class StartupService : IStartupService
{
    private readonly IDataAccess _dataAccess;

    public StartupService(IDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    public async Task InitData()
    {
        await _dataAccess.InitMapPools();
    }
}
