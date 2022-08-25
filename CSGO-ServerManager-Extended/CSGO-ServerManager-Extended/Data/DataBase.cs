using CSGO_ServerManager_Extended.Data.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGO_ServerManager_Extended.Data
{
    public abstract class DataBase
    {
        public readonly IDataAccess _dataAccess;

        public DataBase(IDataAccess db)
        {
            _dataAccess = db;
        }
    }
}
