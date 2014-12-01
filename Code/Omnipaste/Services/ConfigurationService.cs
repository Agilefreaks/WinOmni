﻿namespace Omnipaste.Services
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Reactive.Subjects;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Serialization;
    using OmniCommon;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using OmniCommon.Settings;

    public class ConfigurationService : IConfigurationService
    {
        #region Fields

        private readonly IConfigurationContainer _configurationContainer;

        private readonly Subject<SettingsChangedData> _settingsChangedSubject;

        #endregion

        #region Constructors and Destructors

        public ConfigurationService(IConfigurationContainer configurationContainer)
        {
            _settingsChangedSubject = new Subject<SettingsChangedData>();
            _configurationContainer = configurationContainer;
        }

        #endregion

        #region Public Properties

        public string AccessToken
        {
            get
            {
                return _configurationContainer.GetValue(ConfigurationProperties.AccessToken);
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
                return _configurationContainer.GetValue(ConfigurationProperties.RefreshToken);
            }
        }

        public bool IsSMSSuffixEnabled
        {
            get
            {
                bool result;
                result = !bool.TryParse(_configurationContainer.GetValue(ConfigurationProperties.SMSSuffixEnabled), out result) || result;
                return result;
            }
            set
            {
                _configurationContainer.SetValue(ConfigurationProperties.SMSSuffixEnabled, value.ToString());
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
                var deviceIdentifier = _configurationContainer.GetValue(ConfigurationProperties.DeviceIdentifier);
                if (string.Equals("", deviceIdentifier))
                {
                    deviceIdentifier = Guid.NewGuid().ToString();
                    _configurationContainer.SetValue(ConfigurationProperties.DeviceIdentifier, deviceIdentifier);
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

        public bool DebugMode
        {
            get
            {
                bool value;
                return bool.TryParse(this[ConfigurationProperties.ShowDebugBar], out value) && value;
            }
        }

        public Version Version
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        public string WebBaseUrl
        {
            get
            {
                return this[ConfigurationProperties.WebBaseUrl];
            }
        }

        public IObservable<SettingsChangedData> SettingsChangedObservable
        {
            get
            {
                return _settingsChangedSubject;
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
            _configurationContainer.SetValue(ConfigurationProperties.AccessToken, accessToken);
            _configurationContainer.SetValue(ConfigurationProperties.RefreshToken, refreshToken);
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

                _configurationContainer.SetValue(ConfigurationProperties.ProxyConfiguration, stringWriter.ToString());
                _settingsChangedSubject.OnNext(new SettingsChangedData(ConfigurationProperties.ProxyConfiguration, ProxyConfiguration));
            }
            catch (Exception exception)
            {
                ExceptionReporter.Instance.Report(exception);
            }
        }

        #endregion

        #region Methods

        private ProxyConfiguration GetProxyConfiguration()
        {
            ProxyConfiguration? savedConfiguration = null;

            try
            {
                var serializedConfiguration = _configurationContainer.GetValue(ConfigurationProperties.ProxyConfiguration);
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
                ExceptionReporter.Instance.Report(exception);
            }

            savedConfiguration = savedConfiguration ?? ProxyConfiguration.Empty();

            return savedConfiguration.Value;
        }

        #endregion
    }
}