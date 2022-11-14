using CoreRCON;
using CsgoServerInterface.Exceptions;
using SQLite;

namespace CSGOServerInterface.Server.CsgoServer;

/// <summary>
/// Standard cs:go server, which contains a RCON connection that needs to be created using the InitConnection method.
/// </summary>
[Table("csgo_servers")]
public class CsgoServer : AbstractCsgoServer
{

}