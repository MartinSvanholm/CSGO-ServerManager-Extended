using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGOServerInterface.Server.ServerSettings
{
    public class ServerSettings
    {
        [PrimaryKey, Unique, NotNull, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }
    }
}
