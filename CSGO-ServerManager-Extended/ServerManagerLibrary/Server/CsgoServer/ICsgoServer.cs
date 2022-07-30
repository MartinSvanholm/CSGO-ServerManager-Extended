using CoreRCON;

namespace CsgoServerInterface.CsgoServer
{
    public interface ICsgoServer
    {
        bool Booting { get; set; }
        string ConnectionIp { get; }
        int GamePort { get; set; }
        int? GOTVPort { get; set; }
        int Id { get; set; }
        string Ip { get; set; }
        bool IsFavourite { get; set; }
        bool IsOn { get; set; }
        string Name { get; set; }
        string Password { get; set; }
        RCON Rcon { get; set; }
        string RconPassword { get; set; }
        string Status { get; }
        string DatHostID { get; set; }

        Task GetConnection();
        Task RunCommand(string command, HttpClient httpClient);
        Task StartServer(HttpClient httpClient);
        Task StopServer(HttpClient httpClient);

        void CheckInternetConnection();
    }
}