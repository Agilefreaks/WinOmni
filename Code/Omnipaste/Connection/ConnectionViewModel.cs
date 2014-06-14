namespace Omnipaste.Connection
{
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using Ninject;
    using Omni;
    using OmniSync;

    public class ConnectionViewModel : Screen, IConnectionViewModel
    {
        public IEventAggregator EventAggregator { get; set; }

        public IOmniService OmniService { get; set; }

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
            
            NotifyOfPropertyChange(() => CanConnect);
            NotifyOfPropertyChange(() => CanDisconnect);
        }

        public void Disconnect()
        {
            OmniService.Stop();

            NotifyOfPropertyChange(() => CanConnect);
            NotifyOfPropertyChange(() => CanDisconnect);
        }
    }
}