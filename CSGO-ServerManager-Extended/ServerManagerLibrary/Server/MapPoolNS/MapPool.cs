using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGOServerInterface.Server.MapPoolNS;

public class MapPool
{
    public MapPool(string name, List<Map> maps)
    {
        Name = name;
        Maps = maps;
    }

    public MapPool()
    {

    }

    [PrimaryKey, Unique, NotNull]
    public string Name { get; set; }

    [Ignore]
    public List<Map> Maps { get; set; } = new();

    [Ignore]
    public bool ShowDetials { get; set; }
}
