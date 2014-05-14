using System.Collections.Generic;
using Clipboard;
using Notifications;
using Notifications.NotificationList;
using OmniApi;
using OmniCommon.Framework;
using OmniCommon.Interfaces;
using System.Linq;

namespace Omnipaste.Shell
{
    using System;
    using System.Windows;
    using System.Windows.Interop;
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.EventAggregatorMessages;
    using Configuration;
    using ContextMenu;
    using EventAggregatorMessages;
    using Properties;
    using UserToken;

    public class ShellViewModel : Conductor<IWorkspace>.Collection.OneActive, IShellViewModel
    {
        private Window _view;

        [Inject]
        public IWindowManager WindowManager { get; set; }

        [Inject]
        public IUserTokenViewModel UserTokenViewModel { get; set; }

        [Inject]
        public IKernel Kernel { get; set; }

        [Inject]
        public IContextMenuViewModel ContextMenuViewModel { get; set; }

        public IConfigurationViewModel ConfigurationViewModel { get; set; }

        public ShellViewModel(IConfigurationViewModel configurationViewModel, IEventAggregator eventAggregator)
        {
            ConfigurationViewModel = configurationViewModel;

            DisplayName = Resources.AplicationName;

            eventAggregator.Subscribe(this);
        }

        public void Handle(GetTokenFromUserMessage message)
        {
            ActiveItem = UserTokenViewModel;
        }

        public void Handle(TokenRequestResultMessage message)
        {
            ActiveItem = ConfigurationViewModel;
        }

        public void Handle(ConfigurationCompletedMessage message)
        {
            HandleSuccessfulLogin();
            ActiveItem = ContextMenuViewModel;
            ContextMenuViewModel.Start();
 
            
            var wm = new WindowManager();
            wm.ShowWindow(
                Kernel.Get<INotificationListViewModel>(), 
                null, 
                new Dictionary<string, object>
                {
                    {"Height", SystemParameters.WorkArea.Height},
                    {"Width", SystemParameters.WorkArea.Width}

                });
         
            if (_view != null)
            {
                _view.Visibility = Visibility.Hidden;
                _view.ShowInTaskbar = false;
            }
        }

        public void HandleSuccessfulLogin()
        {
            Kernel.Load(new ClipboardModule(), new DevicesModule(), new NotificationsModule());

            RunStartupTasks();

            var startables = Kernel.GetAll<IStartable>();

            var count = startables.Count();
        }

        protected void RunStartupTasks()
        {
            foreach (var task in Kernel.GetAll<IStartupTask>())
            {
                task.Startup();
            }
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            _view = (Window)view;

            Kernel.Bind<IntPtr>().ToMethod(context => GetHandle());
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            ActiveItem = ConfigurationViewModel;
            
            await ConfigurationViewModel.Start();
        }

        private IntPtr GetHandle()
        {
            var handle = new IntPtr();
            Execute.OnUIThread(() =>
            {
                var windowInteropHelper = new WindowInteropHelper(_view);
                handle = windowInteropHelper.Handle;
            });

            return handle;
        }
    }
}