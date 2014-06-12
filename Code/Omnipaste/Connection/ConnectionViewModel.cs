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

    public class ConnectionViewModel : Screen, IConnectionViewModel
    {
        public IEventAggregator EventAggregator { get; set; }

        public IOmniService OmniService { get; set; }

        [Inject]
        public IKernel Kernel { get; set; }

        public ConnectionViewModel(IEventAggregator eventAggregator, IOmniService omniService)
        {
            OmniService = omniService;
            
            EventAggregator = eventAggregator;
            EventAggregator.Subscribe(this);
        }

        public async Task Handle(ConfigurationCompletedMessage message)
        {
            HandleSuccessfulLogin();

            await OmniService.Start();
        }

        public void HandleSuccessfulLogin()
        {
            Kernel.Load(new ClipboardModule(), new NotificationsModule());
            Kernel.Get<IOmniServiceHandler>().Init();

            var startables = Kernel.GetAll<IStartable>();
            var count = startables.Count();

            var wm = new WindowManager();
            wm.ShowWindow(
                Kernel.Get<INotificationListViewModel>(),
                null,
                new Dictionary<string, object>
                {
                    { "Height", SystemParameters.WorkArea.Height },
                    { "Width", SystemParameters.WorkArea.Width }
                });
        }

        public void Connect()
        {
            EventAggregator.PublishOnCurrentThread(new StartOmniServiceMessage());
        }
    }
}