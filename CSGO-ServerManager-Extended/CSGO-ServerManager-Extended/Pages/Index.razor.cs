using CsgoServerInterface.CsgoServer;

namespace CSGO_ServerManager_Extended.Pages
{
    partial class Index
    {
        private List<ICsgoServer> csgoServers;

        protected override async Task OnInitializedAsync()
        {
            csgoServers = await CsgoServerRepository.GetCsgoServerByCondition(s => s.IsFavourite);
        }
    }
}
