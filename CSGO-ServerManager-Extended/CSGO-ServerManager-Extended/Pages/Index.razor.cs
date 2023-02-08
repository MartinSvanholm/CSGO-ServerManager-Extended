using CSGO_ServerManager_Extended.Models.Constants;
using CSGO_ServerManager_Extended.Services.AccountService;
using CSGO_ServerManager_Extended.Services.CsgoServerService;
using CSGO_ServerManager_Extended.Services.SettingsService;
using CSGO_ServerManager_Extended.Services.StartupService;
using CSGOServerInterface.Server.CsgoServer;
using Microsoft.AspNetCore.Components;

namespace CSGO_ServerManager_Extended.Pages
{
    partial class Index
    {
        [Inject]
        private ICsgoServerService _csgoServerService { get; set; }

        [Inject]
        public ISettingsService _settingsService { get; set; }

        [Inject]
        private IStartupService _startupService { get; set; }

        [Inject]
        private IAccountService accountService { get; set; }

        private List<ICsgoServer> csgoServers = new();

        private bool WelcomeDialogIsVisible { get; set; } = false;
        private bool isLoginDialogVisible { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            if(!_settingsService.UsePasswordLogin || accountService.IsPasswordValidated)
            {
                isLoginDialogVisible = false;
                await InitPage();
            }
        }

        private async Task InitPage()
        {
            if (_settingsService.DashboardVisibilitySetting == SettingsConstants.ShowFavouritesOnDashboard)
                await GetFavouriteServers();

            await _startupService.InitData();

            GetWelcomeMessageVisibility();
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

        private void DisableWelcomeMessage()
        {
            _settingsService.SetWelcomeMessageVisibility(false);
        }

        private void GetWelcomeMessageVisibility()
        {
            WelcomeDialogIsVisible = _settingsService.GetWelcomeMessageVisibility();
        }

        private async Task OnLoginDialogVisibilityChanged()
        {
            isLoginDialogVisible = !isLoginDialogVisible;
            await InitPage();
        }
    }
}
