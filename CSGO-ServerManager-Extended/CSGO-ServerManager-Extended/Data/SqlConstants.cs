using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGO_ServerManager_Extended.Data
{
    public static class SqlConstants
    {
        public static string CreateMapsTable => "create table Maps\r\n(\r\n    DisplayName TEXT not null,\r\n    MapName     TEXT not null\r\n        constraint MapName\r\n            primary key\r\n);\r\n\r\n";

        public static string InsertIntoMapPoolMap => "insert into MapPoolMap(MapPoolname, MapDisplayName) values(?, ?)";

        public static string GetMapsByMapPool => "select Map.DisplayName, Map.MapName from MapPoolMap left join Map on Map.DisplayName = MapPoolMap.MapDisplayName where MapPoolname = ?";
    }
}
