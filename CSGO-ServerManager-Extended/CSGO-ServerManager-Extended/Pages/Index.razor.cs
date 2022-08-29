using CsgoServerInterface.CsgoServer;

namespace CSGO_ServerManager_Extended.Pages
{
    partial class Index
    {
        private List<ICsgoServer> csgoServers;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                csgoServers = new(await CsgoServerRepository.GetCsgoServerByCondition(s => s.ServerSettings.IsFavourite));
            }
            catch (Exception e)
            {
                _snackbar.Add($"Something went wrong: {e.Message}");
            }
        }
    }
}
