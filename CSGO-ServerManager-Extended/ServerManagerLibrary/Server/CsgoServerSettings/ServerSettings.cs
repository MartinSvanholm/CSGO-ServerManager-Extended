using SQLite;

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

        [Column("is_favourite")]
        public bool IsFavourite { get; set; }

        [Column("description")]
        public string Description { get; set; }

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
