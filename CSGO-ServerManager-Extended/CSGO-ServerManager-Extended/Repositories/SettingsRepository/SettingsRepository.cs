using CSGO_ServerManager_Extended.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGO_ServerManager_Extended.Repositories.SettingsRepository
{
    public interface ISettingsRepository
    {
        bool GetUsePassordloginSetting();
        void SaveUsePassordloginSetting(bool usePasswordLoginSetting);
    }

    public class SettingsRepository : ISettingsRepository
    {
        public bool GetUsePassordloginSetting()
        {
            return Preferences.Get(SettingsConstants.UsePasswordLoginKey, false);
        }

        public void SaveUsePassordloginSetting(bool usePasswordLoginSetting)
        {
            Preferences.Set(SettingsConstants.UsePasswordLoginKey, usePasswordLoginSetting);
        }
    }
}
