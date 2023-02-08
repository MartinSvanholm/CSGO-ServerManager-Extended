using CSGO_ServerManager_Extended.Models;
using CSGO_ServerManager_Extended.Services.AccountService;
using CSGO_ServerManager_Extended.Services.SettingsService;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CSGO_ServerManager_Extended.Pages.Settings
{
    public partial class Settings
    {
        [Inject]
        private ISettingsService settingsService { get; set; }

        [Inject]
        private IAccountService _accountService { get; set; }

        [Inject]
        ISnackbar snackbar { get; set; }

        private string TempDashboardVisibilitySetting { get; set; }
        private GlobalServerSettings GlobalServerSettings { get; set; } = new();
        private string Password { get; set; }
        private bool UsePasswordLogin { get => settingsService.UsePasswordLogin; set => settingsService.UsePasswordLogin = value; }


        protected override async Task OnInitializedAsync()
        {
            TempDashboardVisibilitySetting = await settingsService.LoadDashboardVisibilitySetting();

            GlobalServerSettings = settingsService.GetGlobalServerSettings();
        }

        private async Task SubmitDathostAccount()
        {
            try
            {
                settingsService.DathostAccount = await settingsService.AddDathostAccount(settingsService.DathostAccount);
                snackbar.Add("Dathost account linked", Severity.Success, config => { config.Onclick = snackbar => { return Task.CompletedTask; }; });
            }
            catch (Exception e)
            {
                snackbar.Add(e.Message, Severity.Error, config => { config.Onclick = snackbar => { return Task.CompletedTask; }; });
            }
        }

        private void ResetDathostAccount()
        {
            settingsService.RemoveDathostAccount();

            snackbar.Add("Deleted Dathost account", Severity.Success, config => { config.Onclick = snackbar => { return Task.CompletedTask; }; });
        }

        private async Task SaveDashboardVisibilitySetting()
        {
            try
            {
                await settingsService.ChangeDashboardVisibilitySetting(TempDashboardVisibilitySetting);
                snackbar.Add("Saved", Severity.Success, config => { config.Onclick = snackbar => { return Task.CompletedTask; }; });
            }
            catch (Exception e)
            {
                snackbar.Add($"Something went wrong {e.Message}", Severity.Warning, config => { config.Onclick = snackbar => { return Task.CompletedTask; }; });
            }
        }

        private void SaveGlobalServerSettings()
        {
            try
            {
                settingsService.SaveGlobalServerSettings(GlobalServerSettings);
                snackbar.Add("Saved", Severity.Success, config => { config.Onclick = snackbar => { return Task.CompletedTask; }; });
            }
            catch (Exception e)
            {
                snackbar.Add($"Something went wrong {e.Message}", Severity.Warning, config => { config.Onclick = snackbar => { return Task.CompletedTask; }; });
            }
        }

        private void DarkModeChanged()
        {
            settingsService.UseDarkMode = !settingsService.UseDarkMode;
            settingsService.OnUseDarkModeChanged(new EventArgs());
        }

        private void UsePasswordLoginChanged()
        {
            try
            {
                UsePasswordLogin= !UsePasswordLogin;
                settingsService.SaveUsePasswordLoginSetting(UsePasswordLogin);
            }
            catch (Exception e)
            {
                snackbar.Add($"Something went wrong {e.Message}", Severity.Warning, config => { config.Onclick = snackbar => { return Task.CompletedTask; }; });
            }
        }

        private async Task SavePassword()
        {
            try
            {
                await _accountService.SavePassword(Password);
            }
            catch (Exception e)
            {
                snackbar.Add(e.Message, Severity.Error, config => { config.Onclick = snackbar => { return Task.CompletedTask; }; });
            }
        }
    }
}
