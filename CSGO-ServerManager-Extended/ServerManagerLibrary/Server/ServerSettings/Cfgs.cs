using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGOServerInterface.Server.ServerSettings
{
    public class Cfgs
    {
        [PrimaryKey, Unique, NotNull, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("match_cfg")]
        public string MatchCfg { get; set; }

        [Column("overtime_cfg")]
        public string OvertimeCfg { get; set; }

        [Column("knife_cfg")]
        public string KnifeCfg { get; set; }

        [Column("practice_cfg")]
        public string PracticeCfg { get; set; }
    }
}
