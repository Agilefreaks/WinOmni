namespace Omnipaste.Loading.ConnectionTroubleshooter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Caliburn.Micro;
    using OmniCommon;
    using OmniCommon.EventAggregatorMessages;
    using OmniCommon.Interfaces;

    public class ConnectionTroubleshooterViewModel : Screen, IConnectionTroubleshooterViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly IApplicationService _applicationService;

        private readonly IConfigurationService _configurationService;

        private ProxyTypeEnum _proxyType;

        private string _proxyHost;

        private int _proxyPort;

        private string _proxyUsername;

        private string _proxyPassword;

        public IEnumerable<ProxyTypeEnum> ProxyTypes
        {
            get
            {
                return Enum.GetValues(typeof(ProxyTypeEnum)).Cast<ProxyTypeEnum>();
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

        public ConnectionTroubleshooterViewModel(
            IEventAggregator eventAggregator,
            IApplicationService applicationService,
            IConfigurationService configurationService)
        {
            _eventAggregator = eventAggregator;
            _applicationService = applicationService;
            _configurationService = configurationService;
            var existingProxyConfiguration = configurationService.ProxyConfiguration;
            ProxyType = existingProxyConfiguration.Type;
            ProxyHost = existingProxyConfiguration.Address;
            ProxyPort = existingProxyConfiguration.Port;
            ProxyUsername = existingProxyConfiguration.Username;
            ProxyPassword = existingProxyConfiguration.Password;
        }

        public void Exit()
        {
            _applicationService.ShutDown();
        }

        public void Retry()
        {
            _configurationService.SaveProxyConfiguration(
                new ProxyConfiguration
                    {
                        Type = ProxyType,
                        Address = ProxyHost,
                        Port = ProxyPort,
                        Username = ProxyUsername,
                        Password = ProxyPassword
                    });
            _eventAggregator.PublishOnUIThread(new RetryMessage());
        }
    }
}