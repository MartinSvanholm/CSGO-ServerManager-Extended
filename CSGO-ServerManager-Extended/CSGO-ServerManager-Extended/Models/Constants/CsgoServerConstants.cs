using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGO_ServerManager_Extended.Models.Constants
{
    public static class CsgoServerConstants
    {
        public static string NormalCsgoServerType
        {
            get
            {
                return "NormalServer";
            }
        }

        public static string DathostCsgoServerType
        {
            get
            {
                return "DathostServer";
            }
        }

        public static string DefaultMatchCommand => "exec esl5on5.cfg";
        public static string DefaultOvertimeMatchCommand => "exec esl5on5Overtime.cfg";
        public static string DefaultKnifeCommand => "exec knife.cfg";
        public static string DefaultPracticeCommand => "exec practice.cfg";
    }
}
