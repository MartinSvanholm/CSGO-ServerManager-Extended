using CSGO_ServerManager_Extended.Models;
using CSGO_ServerManager_Extended.Models.Constants;
using System.Net.Http.Headers;
using System.Text;

namespace CSGO_ServerManager_Extended.Services.SettingsService;

public interface ISettingsService
{
    HttpClient _httpClient { get; set; }
    string DashboardVisibilitySetting { get; set; }
    DathostAccount DathostAccount { get; set; }
    bool DathostAccountIsConnected { get; set; }
    bool UseDarkMode { get; set; }

    public event EventHandler UseDarkModeChanged;

    Task<DathostAccount> AddDathostAccount(DathostAccount dathostAccount);
    Task ChangeDashboardVisibilitySetting(string dashboardVisibilitySetting);
    Task<string> LoadDashboardVisibilitySetting();
    void RemoveDathostAccount();
    void SaveGlobalServerSettings(GlobalServerSettings globalServerSettings);
    GlobalServerSettings GetGlobalServerSettings();
    void OnUseDarkModeChanged(EventArgs e);
}

public class SettingsService : ISettingsService
{
    public SettingsService(HttpClient httpClient, DathostAccount dathostAccount, bool dathostAccountIsConnected)
    {
        _httpClient = httpClient;
        DathostAccountIsConnected = dathostAccountIsConnected;
        DathostAccount = dathostAccount;

        Task init = Task.Run(() => Init());
        init.Wait();
    }

    public HttpClient _httpClient { get; set; }
    public bool DathostAccountIsConnected { get; set; } = false;
    public DathostAccount DathostAccount { get; set; }
    public string DashboardVisibilitySetting { get; set; }
    public bool UseDarkMode { get; set; }
    public event EventHandler UseDarkModeChanged;

    private async Task Init()
    {
        DashboardVisibilitySetting = await LoadDashboardVisibilitySetting();

        if (DashboardVisibilitySetting == null)
            DashboardVisibilitySetting = SettingsConstants.ShowFavouritesOnDashboard;

        await ChangeDashboardVisibilitySetting(DashboardVisibilitySetting);

        UseDarkMode = Preferences.Get(SettingsConstants.UseDarkMode, false);
    }

    public async Task<DathostAccount> AddDathostAccount(DathostAccount dathostAccount)
    {
        try
        {
            await CheckDathostAccount(dathostAccount);

            await SecureStorage.Default.SetAsync("Dathost_Email", dathostAccount.Email);
            await SecureStorage.Default.SetAsync("Dathost_Password", dathostAccount.Password);

            DathostAccountIsConnected = true;
        }
        catch (Exception)
        {
            throw;
        }

        return dathostAccount;
    }

    public void RemoveDathostAccount()
    {
        SecureStorage.Default.Remove("Dathost_Email");
        SecureStorage.Default.Remove("Dathost_Password");
        DathostAccount = new("", "");
        DathostAccountIsConnected = false;
        _httpClient.DefaultRequestHeaders.Clear();
    }

    public async Task ChangeDashboardVisibilitySetting(string dashboardVisibilitySetting)
    {
        await SecureStorage.Default.SetAsync("DashboardVisibilitySetting", dashboardVisibilitySetting);
        DashboardVisibilitySetting = dashboardVisibilitySetting;
    }

    public async Task<string> LoadDashboardVisibilitySetting()
    {
        return await SecureStorage.Default.GetAsync("DashboardVisibilitySetting");
    }

    private async Task CheckDathostAccount(DathostAccount dathostAccount)
    {
        string uri = _httpClient.BaseAddress.ToString() + "/api/0.1/account";

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{dathostAccount.Email}:{dathostAccount.Password}")));

        using HttpResponseMessage responseMessage = await _httpClient.GetAsync(uri);

        if (!responseMessage.IsSuccessStatusCode)
            throw new HttpRequestException(responseMessage.ReasonPhrase, null, responseMessage.StatusCode);
    }

    public void SaveGlobalServerSettings(GlobalServerSettings globalServerSettings)
    {
        Preferences.Set(GlobalServerCommandsConstants.MatchCommand, globalServerSettings.MatchCommand);
        Preferences.Set(GlobalServerCommandsConstants.OvertimeCommand, globalServerSettings.OvertimeCommand);
        Preferences.Set(GlobalServerCommandsConstants.KnifeCommand, globalServerSettings.KnifeCommand);
        Preferences.Set(GlobalServerCommandsConstants.PracticeCommand, globalServerSettings.PracticeCommand);
    }

    public GlobalServerSettings GetGlobalServerSettings()
    {
        GlobalServerSettings globalServerSettings = new();

        globalServerSettings.MatchCommand = Preferences.Get(GlobalServerCommandsConstants.MatchCommand, null);
        globalServerSettings.OvertimeCommand = Preferences.Get(GlobalServerCommandsConstants.OvertimeCommand, null);
        globalServerSettings.KnifeCommand = Preferences.Get(GlobalServerCommandsConstants.KnifeCommand, null);
        globalServerSettings.PracticeCommand = Preferences.Get(GlobalServerCommandsConstants.PracticeCommand, null);

        return globalServerSettings;
    }

    /// <summary>
    /// Invokes an event to change the state of the MainLayout page. This is needed to change to dark mode immediately.
    /// </summary>
    /// <param name="e"></param>
    public void OnUseDarkModeChanged(EventArgs e)
    {
        Preferences.Set(SettingsConstants.UseDarkMode, UseDarkMode);

        UseDarkModeChanged?.Invoke(this, e);
    }
}