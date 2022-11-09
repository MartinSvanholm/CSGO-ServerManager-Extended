using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGOServerInterface.Server.MapPoolNS
{
    public class MapPoolMap
    {
        [NotNull]
        public string MapPoolname { get; set; }

        [NotNull]
        public string MapDisplayName { get; set; }
    }
}
