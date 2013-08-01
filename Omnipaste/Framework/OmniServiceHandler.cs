using Caliburn.Micro;
using OmniCommon.EventAggregatorMessages;
using OmniCommon.Interfaces;
using Omnipaste.Services.Connectivity;

namespace Omnipaste.Framework
{
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
            _eventAggregator.Subscribe(this);

            _connectivityNotifyService = connectivityNotifyService;
            _connectivityNotifyService.ConnectivityChanged += ConnectivityChanged;
        }

        public void Handle(StartOmniServiceMessage message)
        {
            _omniService.Start();
            _isSyncing = true;
        }

        public void Handle(StopOmniServiceMessage message)
        {
            _omniService.Stop();
            _isSyncing = false;
        }

        private void ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (_isSyncing && e.IsConnected)
            {
                _omniService.Start();
            }

            if (_isSyncing && !e.IsConnected)
            {
                _omniService.Stop();
            }
        }
    }
}
