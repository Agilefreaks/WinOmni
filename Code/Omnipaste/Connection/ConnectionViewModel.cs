namespace Omnipaste.Connection
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using Caliburn.Micro;
    using Clipboard;
    using Ninject;
    using Notifications;
    using Omni;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Framework;
    using Omnipaste.NotificationList;
    using OmniSync;

    public class ConnectionViewModel : Screen, IConnectionViewModel
    {
        public IEventAggregator EventAggregator { get; set; }

        public IOmniService OmniService { get; set; }

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
    }
}