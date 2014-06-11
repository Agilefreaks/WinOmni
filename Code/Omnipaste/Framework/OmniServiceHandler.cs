namespace Omnipaste.Framework
{
    using System;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using Omni;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.Services.Connectivity;

    public class OmniServiceHandler : IOmniServiceHandler
    {
        #region Fields

        private readonly IConnectivityNotifyService _connectivityNotifyService;

        private readonly IEventAggregator _eventAggregator;

        private readonly IOmniService _omniService;

        private bool _isSyncing;

        #endregion

        #region Constructors and Destructors

        public OmniServiceHandler(
            IOmniService omniService,
            IEventAggregator eventAggregator,
            IConnectivityNotifyService connectivityNotifyService)
        {
            _omniService = omniService;
            _eventAggregator = eventAggregator;
            _connectivityNotifyService = connectivityNotifyService;
        }

        #endregion

        #region Public Methods and Operators

        public void Handle(StartOmniServiceMessage message)
        {
            _omniService.Start().ContinueWith(r => _isSyncing = r.Result);
        }

        public void Handle(StopOmniServiceMessage message)
        {
            _omniService.Stop();
            _isSyncing = false;
        }

        public void Init()
        {
            _eventAggregator.Subscribe(this);
            _connectivityNotifyService.ConnectivityChanged += ConnectivityChanged;
        }

        #endregion

        #region Methods

        private async void ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (_isSyncing && e.IsConnected)
            {
                _isSyncing = await _omniService.Start();
            }
            else if (_isSyncing && !e.IsConnected)
            {
                _omniService.Stop();
                _isSyncing = false;
            }
        }

        #endregion
    }
}