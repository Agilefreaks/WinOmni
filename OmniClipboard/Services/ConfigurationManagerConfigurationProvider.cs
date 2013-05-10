using System;
using System.Configuration;

namespace Omniclipboard.Services
{
    public class ConfigurationManagerConfigurationProvider : IConfigurationProvider
    {
        public string GetValue(string key)
        {
            return ConfigurationManager.AppSettings.Get(key);
        }

        public bool SetValue(string key, string value)
        {
            var successfull = true;
            try
            {
                ConfigurationManager.AppSettings.Set(key, value);
            }
            catch (Exception)
            {
                successfull = false;
            }

            return successfull;
        }
    }
}