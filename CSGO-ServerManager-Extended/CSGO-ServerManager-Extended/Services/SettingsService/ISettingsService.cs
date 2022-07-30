using CSGO_ServerManager_Extended.Models;

namespace CSGO_ServerManager_Extended.Services.SettingsService
{
    public interface ISettingsService
    {
        HttpClient _httpClient { get; set; }
        DathostAccount DathostAccount { get; set; }
        bool DathostAccountIsConnected { get; set; }

        Task<DathostAccount> AddDathostAccount(DathostAccount dathostAccount);
        void RemoveDathostAccount();
    }
}