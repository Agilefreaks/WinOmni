namespace Omnipaste.Services.Monitors.ProxyConfiguration
{
    using System;
    using System.Reactive.Subjects;
    using OmniCommon;
    using OmniCommon.Interfaces;

    public class ProxyConfigurationMonitor : IProxyConfigurationMonitor
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        private readonly ReplaySubject<ProxyConfiguration> _subject;

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
            _configurationService.AddProxyConfigurationObserver(this);
        }

        public void Stop()
        {
            _configurationService.RemoveProxyConfigurationObserver(this);
        }

        #endregion
    }
}