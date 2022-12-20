using CoreRCON;
using CoreRCON.Parsers.Standard;
using CSGOServerInterface.Server.CsgoServerSettings;
using CSGOServerInterface.Server.MapPoolNS;
using SQLite;
using Map = CSGOServerInterface.Server.MapPoolNS.Map;

namespace CSGOServerInterface.Server.CsgoServer
{
    public interface ICsgoServer
    {
        bool Booting { get; set; }
        string ConnectionIp { get; }
        int? GamePort { get; set; }
        int? GOTVPort { get; set; }
        string Id { get; set; }
        string Ip { get; set; }
        bool IsOn { get; set; }
        string Name { get; set; }
        string Password { get; set; }
        RCON Rcon { get; set; }
        string RconPassword { get; set; }
        string Status { get; }
        ServerSettings ServerSettings { get; set; }
        MapPool MapPool { get; set; }
        Map MapBeingPlayed { get; set; }
        int PlayersOnline { get; set; }

        Task<Status> GetConnection();
        Task RunCommand(string command, HttpClient httpClient);
        Task StartServer(HttpClient httpClient);
        Task StopServer(HttpClient httpClient);

        void CheckInternetConnection();
    }
}