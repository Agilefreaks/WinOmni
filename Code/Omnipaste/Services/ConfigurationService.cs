namespace Omnipaste.Services
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using BugFreak;
    using OmniCommon;
    using OmniCommon.DataProviders;
    using OmniCommon.Interfaces;

    public class ConfigurationService : IConfigurationService
    {
        #region Fields

        private readonly IConfigurationContainer _persistentConfigurationContainer;

        private readonly List<IProxyConfigurationObserver> _proxyConfigurationObservers;

        #endregion

        #region Constructors and Destructors

        public ConfigurationService(IConfigurationContainer configurationContainer)
        {
            _proxyConfigurationObservers = new List<IProxyConfigurationObserver>();
            _persistentConfigurationContainer = configurationContainer;
        }

        #endregion

        #region Public Properties

        public string AccessToken
        {
            get
            {
                return _persistentConfigurationContainer.GetValue(ConfigurationProperties.AccessToken);
            }
        }

        public ProxyConfiguration ProxyConfiguration
        {
            get
            {
                return GetProxyConfiguration();
            }
        }

        public string RefreshToken
        {
            get
            {
                return _persistentConfigurationContainer.GetValue(ConfigurationProperties.RefreshToken);
            }
        }

        public bool IsSMSSuffixEnabled
        {
            get
            {
                bool result;
                result = !bool.TryParse(_persistentConfigurationContainer[ConfigurationProperties.SMSSuffixEnabled], out result) || result;
                return result;
            }
            set
            {
                _persistentConfigurationContainer[ConfigurationProperties.SMSSuffixEnabled] = value.ToString();
            }
        }

        public string ClientId
        {
            get
            {
                return this[ConfigurationProperties.ClientId];
            }
        }

        public string DeviceIdentifier
        {
            get
            {
                var deviceIdentifier = _persistentConfigurationContainer.GetValue(ConfigurationProperties.DeviceIdentifier);
                if (string.Equals("", deviceIdentifier))
                {
                    deviceIdentifier = Guid.NewGuid().ToString();
                    _persistentConfigurationContainer.SetValue(ConfigurationProperties.DeviceIdentifier, deviceIdentifier);
                }

                return deviceIdentifier;
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
                return _persistentConfigurationContainer.GetValue(ConfigurationProperties.AutoStart, true);
            }

            set
            {
                _persistentConfigurationContainer.SetValue(ConfigurationProperties.AutoStart, value.ToString());
            }
        }

        public bool DebugMode
        {
            get
            {
                bool value;
                return bool.TryParse(this[ConfigurationProperties.ShowDebugBar], out value) && value;
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
            _persistentConfigurationContainer.SetValue(ConfigurationProperties.AccessToken, accessToken);
            _persistentConfigurationContainer.SetValue(ConfigurationProperties.RefreshToken, refreshToken);
        }

        public void ResetAuthSettings()
        {
            SaveAuthSettings(string.Empty, string.Empty);
        }

        public void SaveProxyConfiguration(ProxyConfiguration value)
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(ProxyConfiguration));
                var stringWriter = new StringWriter();
                xmlSerializer.Serialize(stringWriter, value);

                _persistentConfigurationContainer.SetValue(ConfigurationProperties.ProxyConfiguration, stringWriter.ToString());
                _proxyConfigurationObservers.ForEach(observer => observer.OnConfigurationChanged(ProxyConfiguration));
            }
            catch (Exception exception)
            {
                ReportingService.Instance.BeginReport(exception);
            }
        }

        public void AddProxyConfigurationObserver(IProxyConfigurationObserver proxyConfigurationObserver)
        {
            _proxyConfigurationObservers.Add(proxyConfigurationObserver);
        }

        public void RemoveProxyConfigurationObserver(IProxyConfigurationObserver proxyConfigurationObserver)
        {
            _proxyConfigurationObservers.Remove(proxyConfigurationObserver);
        }

        #endregion

        #region Methods

        private ProxyConfiguration GetProxyConfiguration()
        {
            ProxyConfiguration? savedConfiguration = null;

            try
            {
                var serializedConfiguration = _persistentConfigurationContainer.GetValue(ConfigurationProperties.ProxyConfiguration);
                if (!string.IsNullOrWhiteSpace(serializedConfiguration))
                {
                    var xmlSerializer = new XmlSerializer(typeof(ProxyConfiguration));
                    var stringReader = new StringReader(serializedConfiguration);
                    var xmlReader = XmlReader.Create(stringReader);
                    if (xmlSerializer.CanDeserialize(xmlReader))
                    {
                        savedConfiguration = (ProxyConfiguration)xmlSerializer.Deserialize(xmlReader);
                    }
                }
            }
            catch (Exception exception)
            {
                ReportingService.Instance.BeginReport(exception);
            }

            savedConfiguration = savedConfiguration ?? ProxyConfiguration.Empty();

            return savedConfiguration.Value;
        }

        #endregion
    }
}