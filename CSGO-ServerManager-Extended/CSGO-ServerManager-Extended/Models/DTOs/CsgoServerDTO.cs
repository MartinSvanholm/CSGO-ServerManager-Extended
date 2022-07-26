using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGO_ServerManager_Extended.Models.DTOs
{
    [Table("CsgoServers")]
    public class CsgoServerDTO
    {
        [PrimaryKey, Unique, NotNull]
        [Column("name")]
        public string Name { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("rcon_password")]
        public string RconPassord { get; set; }

        [NotNull]
        [Column("ip")]
        public string Ip { get; set; }

        [NotNull]
        [Column("game_port")]
        public int GamePort { get; set; }

        [Column("gotv_port")]
        public int GOTVPort { get; set; }
    }
}
