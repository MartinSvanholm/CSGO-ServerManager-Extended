using CSGO_ServerManager_Extended.Services.CsgoServerService;
using CSGO_ServerManager_Extended.Services.Data.CsgoServerData;
using CsgoServerInterface.CsgoServer;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CSGO_ServerManager_Extended.Pages
{
    partial class Index
    {
        private List<ICsgoServer> csgoServers;

        protected override async Task OnInitializedAsync()
        {
            var data = await CsgoServerData.CsgoServers_GetAll();
            csgoServers = data.Where(s => s.IsFavourite).ToList();
        }
    }
}
