namespace Omnipaste.Connection
{
    using System;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using Omni;
    using OmniSync;

    public class ConnectionViewModel : Screen, IConnectionViewModel
    {
        #region Fields

        private bool _enabled = true;

        private IOmniService _omniService;

        private IDisposable _omniServiceStatusObserver;

        #endregion

        #region Constructors and Destructors

        public ConnectionViewModel(IOmniService omniService)
        {
            OmniService = omniService;
        }

        #endregion

        #region Public Properties

        public bool CanConnect
        {
            get
            {
                return !Connected;
            }
        }

        public bool CanDisconnect
        {
            get
            {
                return Connected;
            }
        }

        public bool Connected
        {
            get
            {
                return OmniService.Status == ServiceStatusEnum.Started;
            }
        }

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
        
        public IEventAggregator EventAggregator { get; set; }

        public bool IsConnected
        {
            get
            {
                return OmniService.Status == ServiceStatusEnum.Started;
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

        #endregion

        #region Public Methods and Operators

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

        #endregion
    }
}