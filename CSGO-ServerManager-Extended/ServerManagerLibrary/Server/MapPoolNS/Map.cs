using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGOServerInterface.Server.MapPoolNS;

public class Map
{
    public Map(string displayName, string mapName)
    {
        DisplayName = displayName;
        MapName = mapName;
    }

    public Map()
    {

    }

    [PrimaryKey, Unique, NotNull]
    public string DisplayName { get; set; }

    [NotNull]
    public string MapName { get; set; }

    [Ignore]
    public bool IsNew { get; set; }
}
