namespace Omnipaste.Services
{
    using System;
    using System.Configuration;
    using System.Reactive.Subjects;
    using System.Reflection;
    using OmniCommon;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using OmniCommon.Settings;
    using Omnipaste.ExtensionMethods;

    public class ConfigurationService : IConfigurationService
    {
        #region Fields

        private readonly IConfigurationContainer _configurationContainer;

        private readonly Subject<SettingsChangedData> _settingsChangedSubject;

        private bool _deviceIdentifierChanged;

        #endregion

        #region Constructors and Destructors

        public ConfigurationService(IConfigurationContainer configurationContainer)
        {
            _settingsChangedSubject = new Subject<SettingsChangedData>();
            _configurationContainer = configurationContainer;
            _deviceIdentifierChanged = false;
        }

        #endregion

        #region Public Properties

        public ProxyConfiguration ProxyConfiguration
        {
            get
            {
                return _configurationContainer.GetObject<ProxyConfiguration>(ConfigurationProperties.ProxyConfiguration);
            }

            set
            {
                _configurationContainer.SetObject(ConfigurationProperties.ProxyConfiguration, value);
                _settingsChangedSubject.OnNext(new SettingsChangedData(ConfigurationProperties.ProxyConfiguration, ProxyConfiguration));
            }
        }

        public UserInfo UserInfo
        {
            get
            {
                return _configurationContainer.GetObject<UserInfo>(ConfigurationProperties.UserInfo);
            }

            set
            {
                _configurationContainer.SetObject(ConfigurationProperties.UserInfo, value);
                _settingsChangedSubject.OnNext(new SettingsChangedData(ConfigurationProperties.UserInfo, UserInfo));
            }
        }

        public string AccessToken
        {
            get
            {
                return _configurationContainer.GetValue(ConfigurationProperties.AccessToken);
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
                _settingsChangedSubject.OnNext(new SettingsChangedData(ConfigurationProperties.SMSSuffixEnabled, value));
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
                return _configurationContainer.GetValue(ConfigurationProperties.DeviceIdentifier);
            }

            set
            {
                _configurationContainer.SetValue(ConfigurationProperties.DeviceIdentifier, value);
                _deviceIdentifierChanged = true;
            }
        }

        public KeyPair DeviceKeyPair
        {
            get
            {
                return _configurationContainer.GetObject<KeyPair>(ConfigurationProperties.DeviceKeyPair);
            }

            set
            {
                _configurationContainer.SetObject(ConfigurationProperties.DeviceKeyPair, value);
                _settingsChangedSubject.OnNext(new SettingsChangedData(ConfigurationProperties.DeviceKeyPair, DeviceKeyPair));
            }
        }

        public bool IsNewDevice
        {
            get
            {
                return string.IsNullOrWhiteSpace(DeviceIdentifier) || _deviceIdentifierChanged;
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

        public string AppNameAndVersion
        {
            get
            {
                return string.Format("{0} {1}", Constants.AppName, Version);
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

        public void SaveAuthSettings(OmnipasteCredentials omnipasteCredentials)
        {
            _configurationContainer.SetValue(ConfigurationProperties.AccessToken, omnipasteCredentials.AccessToken);
            _configurationContainer.SetValue(ConfigurationProperties.RefreshToken, omnipasteCredentials.RefreshToken);
            _settingsChangedSubject.OnNext(new SettingsChangedData(ConfigurationProperties.OmnipasteCredentials, omnipasteCredentials));
        }

        public void ResetAuthSettings()
        {
            SaveAuthSettings(new OmnipasteCredentials());
        }

        public bool HasSavedValueFor(string propertyName)
        {
            return _configurationContainer.HasValue(propertyName);
        }

        #endregion
    }
}