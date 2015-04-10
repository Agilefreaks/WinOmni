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
    using Omnipaste.Framework.ExtensionMethods;

    public class ConfigurationService : IConfigurationService
    {
        #region Fields

        private readonly IConfigurationContainer _configurationContainer;

        private readonly Subject<SettingsChangedData> _settingsChangedSubject;

        private bool _deviceIdChanged;

        #endregion

        #region Constructors and Destructors

        public ConfigurationService(IConfigurationContainer configurationContainer)
        {
            _settingsChangedSubject = new Subject<SettingsChangedData>();
            _configurationContainer = configurationContainer;
            _deviceIdChanged = false;
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
                var userInfo = _configurationContainer.GetObject<UserInfo>(ConfigurationProperties.UserInfo);
                return userInfo;
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

        //ToDo: This should be removed once the migration to using the  DeviceId id completed - 15.01.2014
        public string DeviceIdentifier
        {
            get
            {
                return _configurationContainer.GetValue(ConfigurationProperties.DeviceIdentifier);
            }

            set
            {
                _configurationContainer.SetValue(ConfigurationProperties.DeviceIdentifier, value);
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
                return string.IsNullOrWhiteSpace(DeviceId) || _deviceIdChanged;
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

        public string DeviceId
        {
            get
            {
                return _configurationContainer.GetValue(ConfigurationProperties.DeviceId);
            }
            set
            {
                _configurationContainer.SetValue(ConfigurationProperties.DeviceId, value);
                _deviceIdChanged = true;
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

        public bool LoggingEnabled
        {
            get
            {
                bool result;
                return bool.TryParse(this[ConfigurationProperties.EnableLogging], out result) && result;
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

        public void ClearSettings()
        {
            _configurationContainer.ClearAll();
            _settingsChangedSubject.OnNext(new SettingsChangedData(ConfigurationProperties.UserInfo));
            _settingsChangedSubject.OnNext(new SettingsChangedData(ConfigurationProperties.ProxyConfiguration));
        }

        public bool HasSavedValueFor(string propertyName)
        {
            return _configurationContainer.HasValue(propertyName);
        }

        #endregion
    }
}