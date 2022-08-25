using CSGO_ServerManager_Extended.Data.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGO_ServerManager_Extended.Repositories
{
    public class RepositoryBase
    {
        public RepositoryBase(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public IDataAccess _dataAccess { get; set; }
    }
}
