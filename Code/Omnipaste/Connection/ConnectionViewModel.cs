namespace Omnipaste.Connection
{
    using System;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using Ninject;
    using Omni;
    using OmniSync;

    public class ConnectionViewModel : Screen, IConnectionViewModel
    {
        private IOmniService _omniService;

        private IDisposable _omniServiceStatusObserver;

        public IEventAggregator EventAggregator { get; set; }

        public IOmniService OmniService
        {
            get
            {
                return _omniService;
            }
            set
            {
                if (_omniServiceStatusObserver != null)
                {
                    _omniServiceStatusObserver.Dispose();
                }

                _omniService = value;

                _omniServiceStatusObserver = _omniService.Subscribe(
                    x =>
                    {
                        NotifyOfPropertyChange(() => CanConnect);
                        NotifyOfPropertyChange(() => CanDisconnect);
                    });
            }
        }

        public bool CanConnect
        {
            get
            {
                return !IsConnected;
            }
        }

        public bool CanDisconnect
        {
            get
            {
                return IsConnected;
            }
        }

        public bool IsConnected
        {
            get
            {
                return OmniService.Status == ServiceStatusEnum.Started;
            }
        }
        
        [Inject]
        public IKernel Kernel { get; set; }

        public ConnectionViewModel(IOmniService omniService)
        {
            OmniService = omniService;
        }

        public async Task Connect()
        {
            await OmniService.Start();
        }

        public void Disconnect()
        {
            OmniService.Stop();
        }
    }
}