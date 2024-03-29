﻿using SQLite;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace CSGOServerInterface.Server.CsgoServerSettings
{
    [Table("server_settings")]
    public class ServerSettings
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Unique, NotNull]
        [Column("csgo_server_id")]
        public string CsgoServerId { get; set; }

        [NotNull]
        [Column("is_dathostserver")]
        public bool IsDathostServer { get; set; }

        [Column("is_favourite")]
        public bool IsFavourite { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [NotNull]
        public string MapPoolName { get; set; } = "Active duty";

        [Column("match_cfg")]
        public string MatchCommand { get; set; }

        [Column("overtime_cfg")]
        public string OvertimeCommand { get; set; }

        [Column("knife_cfg")]
        public string KnifeCommand { get; set; }

        [Column("practice_cfg")]
        public string PracticeCommand { get; set; }
    }
}
