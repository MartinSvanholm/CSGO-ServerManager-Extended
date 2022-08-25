using CSGO_ServerManager_Extended.Services.SettingsService;
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

        private async Task Submit()
        {
            try
            {
                settingsService.DathostAccount = await settingsService.AddDathostAccount(settingsService.DathostAccount);
                snackbar.Add("Dathost account linked", Severity.Success);
            }
            catch (Exception e)
            {
                snackbar.Add(e.Message, Severity.Error);
            }
        }

        private void Reset()
        {
            settingsService.RemoveDathostAccount();

            snackbar.Add("Deleted Dathost account", Severity.Success);
        }
    }
}
