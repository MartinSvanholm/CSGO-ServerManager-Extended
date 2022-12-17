using CoreRCON;
using CoreRCON.Parsers.Standard;
using CsgoServerInterface.Exceptions;
using CSGOServerInterface.Server.CsgoServerSettings;
using CSGOServerInterface.Server.MapPoolNS;
using SQLite;
using System.Net;
using Map = CSGOServerInterface.Server.MapPoolNS.Map;

namespace CSGOServerInterface.Server.CsgoServer
{
    public abstract class AbstractCsgoServer : ICsgoServer
    {
        [PrimaryKey, Unique, NotNull]
        [Column("id")]
        public string Id { get; set; }

        [NotNull, Unique]
        [Column("name")]
        public string Name { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("rcon_password")]
        public string RconPassword { get; set; }

        [NotNull]
        [Column("ip")]
        public string Ip { get; set; }

        [NotNull]
        [Column("game_port")]
        public int GamePort { get; set; }

        [Column("gotv_port")]
        public int? GOTVPort { get; set; }

        [Ignore]
        public ServerSettings ServerSettings { get; set; } = new();

        /// <summary>
        /// Remote Console connection
        /// </summary>
        [Ignore]
        public RCON Rcon { get; set; }

        [Ignore]
        public bool Booting { get; set; }

        [Ignore]
        public bool IsOn { get; set; }

        [Ignore]
        public string Status
        {
            get
            {
                if (Booting == true)
                    return "booting";
                else if (IsOn)
                    return "online";
                else
                    return "offline";
            }
        }

        [Ignore]
        public string ConnectionIp
        {
            get
            {
                if (string.IsNullOrEmpty(Password.Trim()))
                    return $"connect {Ip.Trim()}:{GamePort}";
                else
                    return $"connect {Ip.Trim()}:{GamePort}; password {Password.Trim()}";
            }
        }

        [Ignore]
        public MapPool MapPool { get; set; }

        [Ignore]
        public Map MapBeingPlayed { get; set; }

        public virtual async Task GetConnection()
        {
            CheckInternetConnection();

            Rcon = new(IPAddress.Parse(Ip), Convert.ToUInt16(GamePort), Password);

            await Rcon.ConnectAsync();
            Status status = await Rcon.SendCommandAsync<Status>("status");
        }

        public virtual async Task RunCommand(string command, HttpClient httpClient)
        {
            try
            {
                string response = await Rcon.SendCommandAsync(command);
            }
            catch (Exception e)
            {
                throw new CsgoServerException(e.Message, this);
            }
        }

        public virtual Task StartServer(HttpClient httpClient)
        {
            throw new NotImplementedException("Can only start DatHostServers, please start the server manually.");
        }

        public virtual Task StopServer(HttpClient httpClient)
        {
            throw new NotImplementedException("Can only stop DatHostServers, please stop the server manually.");
        }

        public void CheckInternetConnection()
        {
            if (!(Connectivity.Current.NetworkAccess == NetworkAccess.Internet))
                throw new CsgoServerException("Device not connected to the internet", this);
        }
    }
}
