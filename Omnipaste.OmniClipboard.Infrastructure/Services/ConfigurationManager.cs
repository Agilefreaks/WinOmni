namespace Omnipaste.OmniClipboard.Infrastructure.Services
{
    using System.Collections.Specialized;

    class ConfigurationManager : IConfigurationManager
    {
        public NameValueCollection AppSettings
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings;
            }
        }
    }
}