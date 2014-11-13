namespace Omnipaste.Loading.ConnectionTroubleshooter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using OmniCommon;
    using OmniCommon.EventAggregatorMessages;
    using OmniCommon.Interfaces;
    using Omnipaste.Services;

    public class ConnectionTroubleshooterViewModel : Screen, IConnectionTroubleshooterViewModel
    {
        #region Fields

        private readonly IApplicationService _applicationService;

        private readonly IConfigurationService _configurationService;

        private readonly IEventAggregator _eventAggregator;

        private readonly INetworkService _networkService;

        private bool _isBusy;

        private string _pingExceptionMessage;

        private string _proxyHost;

        private string _proxyPassword;

        private int _proxyPort;

        private ProxyTypeEnum _proxyType;

        private string _proxyUsername;

        #endregion

        #region Constructors and Destructors

        public ConnectionTroubleshooterViewModel(
            IEventAggregator eventAggregator,
            IApplicationService applicationService,
            IConfigurationService configurationService,
            INetworkService networkService)
        {
            _eventAggregator = eventAggregator;
            _applicationService = applicationService;
            _configurationService = configurationService;
            _networkService = networkService;
            var existingProxyConfiguration = configurationService.ProxyConfiguration;
            ProxyType = existingProxyConfiguration.Type;
            ProxyHost = existingProxyConfiguration.Address;
            ProxyPort = existingProxyConfiguration.Port;
            ProxyUsername = existingProxyConfiguration.Username;
            ProxyPassword = existingProxyConfiguration.Password;
        }

        #endregion

        #region Public Properties

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            private set
            {
                if (value.Equals(_isBusy))
                {
                    return;
                }
                _isBusy = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => CanRetry);
            }
        }

        public bool CanRetry
        {
            get
            {
                return !IsBusy;
            }
        }

        public string PingExceptionMessage
        {
            get
            {
                return _pingExceptionMessage;
            }
            set
            {
                if (Equals(value, _pingExceptionMessage))
                {
                    return;
                }
                _pingExceptionMessage = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => ShowPingExceptionMessage);
            }
        }

        public ProxyConfiguration ProxyConfiguration
        {
            get
            {
                return new ProxyConfiguration
                           {
                               Type = ProxyType,
                               Address = ProxyHost,
                               Port = ProxyPort,
                               Username = ProxyUsername,
                               Password = ProxyPassword
                           };
            }
        }

        public string ProxyHost
        {
            get
            {
                return _proxyHost;
            }
            set
            {
                if (value == _proxyHost)
                {
                    return;
                }
                _proxyHost = value;
                NotifyOfPropertyChange();
            }
        }

        public string ProxyPassword
        {
            get
            {
                return _proxyPassword;
            }
            set
            {
                if (value == _proxyPassword)
                {
                    return;
                }
                _proxyPassword = value;
                NotifyOfPropertyChange();
            }
        }

        public int ProxyPort
        {
            get
            {
                return _proxyPort;
            }
            set
            {
                if (value == _proxyPort)
                {
                    return;
                }
                _proxyPort = value;
                NotifyOfPropertyChange();
            }
        }

        public ProxyTypeEnum ProxyType
        {
            get
            {
                return _proxyType;
            }
            set
            {
                if (value == _proxyType)
                {
                    return;
                }
                _proxyType = value;
                NotifyOfPropertyChange();
            }
        }

        public IEnumerable<ProxyTypeEnum> ProxyTypes
        {
            get
            {
                return Enum.GetValues(typeof(ProxyTypeEnum)).Cast<ProxyTypeEnum>();
            }
        }

        public string ProxyUsername
        {
            get
            {
                return _proxyUsername;
            }
            set
            {
                if (value == _proxyUsername)
                {
                    return;
                }
                _proxyUsername = value;
                NotifyOfPropertyChange();
            }
        }

        public bool ShowPingExceptionMessage
        {
            get
            {
                return !string.IsNullOrWhiteSpace(_pingExceptionMessage);
            }
        }

        public int MaxPortNumber
        {
            get
            {
                return 65535;
            }
        }

        public bool ShowProxySettings
        {
            get
            {
                return ProxyType != ProxyTypeEnum.None;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Exit()
        {
            _applicationService.ShutDown();
        }

        public void Retry()
        {
            IsBusy = true;
            Task.Factory.StartNew(PingWithCurrentConfiguration).ContinueWith(OnPingFinished);
        }

        #endregion

        #region Methods

        private void PingWithCurrentConfiguration()
        {
            _networkService.PingHome(ProxyConfiguration);
        }

        private void OnPingFinished(Task pingTask)
        {
            if (pingTask.Exception != null)
            {
                PingExceptionMessage = pingTask.Exception.ToString();
            }
            else
            {
                _configurationService.SaveProxyConfiguration(ProxyConfiguration);
                _eventAggregator.PublishOnUIThread(new RetryMessage());
            }

            IsBusy = false;
        }

        #endregion
    }
}