namespace Omnipaste.DataProviders
{
    using System.Configuration;
    using OmniCommon.DataProviders;

    public class AppConfigurationProvider : IAppConfigurationProvider
    {
        public string GetValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public bool SetValue(string key, string value)
        {
            throw new System.NotImplementedException();
        }
    }
}
