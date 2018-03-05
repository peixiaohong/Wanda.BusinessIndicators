using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib.Config
{
    public static class AppSettingConfig
    {
        public static string GetSetting(string key, string defaultValue)
        {
            string result = System.Configuration.ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(result))
            {
                result = defaultValue;
            }
            return result;
        }
    }
}
