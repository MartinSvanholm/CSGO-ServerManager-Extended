using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGOServerInterface.Exceptions
{
    public class SmDataException : Exception
    {
        public SmDataException(string message, string objectName, string objectId = null) : base($"{message} {objectName} {(objectId == null ? "" : $"with id {objectId}")}")
        {
            ObjectId = objectId;
            ObjectName = objectName;
        }

        public string ObjectId { get; set; }
        public string ObjectName { get; set; }
    }
}
