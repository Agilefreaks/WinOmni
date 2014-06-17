namespace Omnipaste.Connection
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Controls.Primitives;
    using Caliburn.Micro;
    using Castle.Core;
    using Ninject;
    using Omni;
    using OmniSync;

    public class ConnectionViewModel : Screen, IConnectionViewModel
    {
        private IOmniService _omniService;

        private IDisposable _omniServiceStatusObserver;

        private bool _enabled = true;

        public IEventAggregator EventAggregator { get; set; }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
                NotifyOfPropertyChange(() => Enabled);
            }
        }

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
            Enabled = false;

            await OmniService.Start();

            Enabled = true;
        }

        public void Disconnect()
        {
            Enabled = false;

            OmniService.Stop();

            Enabled = true;
        }
    }
}