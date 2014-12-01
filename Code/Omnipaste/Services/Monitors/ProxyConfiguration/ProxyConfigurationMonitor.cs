namespace Omnipaste.Services.Monitors.ProxyConfiguration
{
    using System;
    using System.Reactive.Subjects;
    using OmniCommon;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Interfaces;

    public class ProxyConfigurationMonitor : IProxyConfigurationMonitor
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        private readonly ReplaySubject<ProxyConfiguration> _subject;

        private IDisposable _changeSubscription;

        #endregion

        #region Constructors and Destructors

        public ProxyConfigurationMonitor(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
            _subject = new ReplaySubject<ProxyConfiguration>();
        }

        #endregion

        #region Public Properties

        public IObservable<ProxyConfiguration> ProxyConfigurationObservable
        {
            get
            {
                return _subject;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void OnConfigurationChanged(ProxyConfiguration proxyConfiguration)
        {
            _subject.OnNext(proxyConfiguration);
        }

        public void Start()
        {
            Stop();
            _changeSubscription =
                _configurationService.SettingsChangedObservable.SubscribeToSettingChange<ProxyConfiguration>(
                    ConfigurationProperties.ProxyConfiguration,
                    OnConfigurationChanged);
        }

        public void Stop()
        {
            if (_changeSubscription == null)
            {
                return;
            }

            _changeSubscription.Dispose();
            _changeSubscription = null;
        }

        #endregion
    }
}