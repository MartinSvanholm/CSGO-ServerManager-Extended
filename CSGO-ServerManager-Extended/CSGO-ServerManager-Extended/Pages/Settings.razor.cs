﻿using CSGO_ServerManager_Extended.Services.SettingsService;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CSGO_ServerManager_Extended.Pages
{
    public partial class Settings
    {
        [Inject]
        private ISettingsService settingsService { get; set; }

        [Inject]
        ISnackbar snackbar { get; set; }

        private string TempDashboardVisibilitySetting { get; set; }

        protected override async Task OnInitializedAsync()
        {
            TempDashboardVisibilitySetting = await settingsService.LoadDashboardVisibilitySetting();
        }

        private async Task Submit()
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

        private void Reset()
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
    }
}
