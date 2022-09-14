using CsgoServerInterface.Exceptions;
using CSGOServerInterface.Server.CsgoServer;
using CSGOServerInterface.Server.CsgoServerSettings;

namespace CSGOServerInterface.Server.DathostServer
{
    public class DatHostCsgoServer : AbstractCsgoServer
    {
        public DatHostCsgoServer(bool booting, int gamePort, int? gOTVPort, string id, string ip, bool isOn, string name, string password, string rconPassword, ServerSettings serverSettings)
        {
            Booting = booting;
            GamePort = gamePort;
            GOTVPort = gOTVPort;
            Id = id;
            Ip = ip;
            IsOn = isOn;
            Name = name;
            Password = password;
            RconPassword = rconPassword;
            ServerSettings = serverSettings;
        }

        public override Task GetConnection()
        {
            throw new NotImplementedException();
        }

        public override async Task RunCommand(string command, HttpClient httpClient)
        {
            CheckInternetConnection();

            string uri = httpClient.BaseAddress.ToString() + $"/api/0.1/game-servers/{Id}/console";

            var data = new Dictionary<string, string>
            {
                { "line", command}
            };

            var content = new FormUrlEncodedContent(data);

            using HttpResponseMessage responseMessage = await httpClient.PostAsync(uri, content);

            if (!responseMessage.IsSuccessStatusCode)
                throw new CsgoServerException(responseMessage.ReasonPhrase, this, responseMessage.StatusCode);
        }

        public override async Task StartServer(HttpClient httpClient)
        {
            CheckInternetConnection();

            string uri = httpClient.BaseAddress.ToString() + $"/api/0.1/game-servers/{Id}/start";

            using HttpResponseMessage responseMessage = await httpClient.PostAsync(uri, null);

            if (!responseMessage.IsSuccessStatusCode)
                throw new CsgoServerException(responseMessage.ReasonPhrase, this, responseMessage.StatusCode);
        }

        public override async Task StopServer(HttpClient httpClient)
        {
            CheckInternetConnection();
            
            string uri = httpClient.BaseAddress.ToString() + $"/api/0.1/game-servers/{Id}/stop";

            using HttpResponseMessage responseMessage = await httpClient.PostAsync(uri, null);

            if (!responseMessage.IsSuccessStatusCode)
                throw new CsgoServerException(responseMessage.ReasonPhrase, this, responseMessage.StatusCode);
        }
    }
}