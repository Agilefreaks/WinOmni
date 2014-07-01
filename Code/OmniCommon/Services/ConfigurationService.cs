namespace OmniCommon.Services
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Net.NetworkInformation;
    using OmniCommon.DataProviders;
    using OmniCommon.Interfaces;

    public class ConfigurationService : IConfigurationService
    {
        #region Fields

        private readonly IConfigurationProvider _configurationProvider;

        #endregion

        #region Constructors and Destructors

        public ConfigurationService(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        #endregion

        #region Public Properties

        public string AccessToken
        {
            get
            {
                return _configurationProvider.GetValue(ConfigurationProperties.AccessToken);
            }
        }

        public string RefreshToken
        {
            get
            {
                return _configurationProvider.GetValue(ConfigurationProperties.RefreshToken);
            }
        }

        public string ClientId
        {
            get
            {
                return ConfigurationManager.AppSettings[ConfigurationProperties.ClientId];
            }
        }

        public string DeviceIdentifier
        {
            get
            {
                return
                   NetworkInterface.GetAllNetworkInterfaces()
                       .Where(nic => nic.OperationalStatus == OperationalStatus.Up)
                       .Select(nic => nic.GetPhysicalAddress().ToString())
                       .First();
            }
        }

        public string MachineName
        {
            get
            {
                return Environment.MachineName;
            }
        }

        public bool AutoStart
        {
            get
            {
                return _configurationProvider.GetValue(ConfigurationProperties.AutoStart, true);
            }

            set
            {
                _configurationProvider.SetValue(ConfigurationProperties.AutoStart, value.ToString());
            }
        }

        #endregion

        #region Public Indexers

        public string this[string key]
        {
            get
            {
                return ConfigurationManager.AppSettings[key];
            }
        }

        #endregion

        #region Public Methods and Operators

        public void SaveAuthSettings(string accessToken, string refreshToken)
        {
            _configurationProvider.SetValue(ConfigurationProperties.AccessToken, accessToken);
            _configurationProvider.SetValue(ConfigurationProperties.RefreshToken, refreshToken);
        }

        public void ResetAuthSettings()
        {
            SaveAuthSettings(string.Empty, string.Empty);
        }

        #endregion
    }
}