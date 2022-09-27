using CSGO_ServerManager_Extended.Models.Constants;
using CSGO_ServerManager_Extended.Services.CsgoServerService;
using CSGO_ServerManager_Extended.Services.SettingsService;
using CsgoServerInterface.CsgoServer;
using Microsoft.AspNetCore.Components;

namespace CSGO_ServerManager_Extended.Pages
{
    partial class Index
    {
        [Inject]
        private ICsgoServerService _csgoServerService { get; set; }

        [Inject]
        public ISettingsService _settingsService { get; set; }

        private List<ICsgoServer> csgoServers = new();

        protected override async Task OnInitializedAsync()
        {
            if(_settingsService.DashboardVisibilitySetting == SettingsConstants.ShowFavouritesOnDashboard)
                await GetFavouriteServers();
        }

        private async Task GetFavouriteServers()
        {
            try
            {
                csgoServers.AddRange( await _csgoServerService.GetFavouriteCsgoServers());
            }
            catch (Exception e)
            {
                _snackbar.Add(e.Message, MudBlazor.Severity.Warning, config => { config.Onclick = snackbar => { return Task.CompletedTask; }; });
            }
        }
    }
}
