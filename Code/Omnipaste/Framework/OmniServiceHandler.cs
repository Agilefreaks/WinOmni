namespace Omnipaste.Framework
{
    using Caliburn.Micro;
    using Omni;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.Services.Connectivity;

    public class OmniServiceHandler : IOmniServiceHandler
    {
        private readonly IOmniService _omniService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IConnectivityNotifyService _connectivityNotifyService;
        private bool _isSyncing;

        public OmniServiceHandler(IOmniService omniService, IEventAggregator eventAggregator, IConnectivityNotifyService connectivityNotifyService)
        {
            _omniService = omniService;
            _eventAggregator = eventAggregator;
            _connectivityNotifyService = connectivityNotifyService;
        }

        public void Init()
        {
            _eventAggregator.Subscribe(this);
            _connectivityNotifyService.ConnectivityChanged += ConnectivityChanged;
        }

        public async void Handle(StartOmniServiceMessage message)
        {
            _isSyncing = await _omniService.Start();
        }

        public void Handle(StopOmniServiceMessage message)
        {
            _omniService.Stop();
            _isSyncing = false;
        }

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
    }
}
