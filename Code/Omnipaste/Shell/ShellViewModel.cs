namespace Omnipaste.Shell
{
    using System.Collections.Generic;
    using System.Deployment.Application;
    using System.Reflection;
    using Clipboard;
    using Notifications;
    using OmniApi;
    using OmniCommon.Framework;
    using OmniCommon.Interfaces;
    using System.Linq;
    using Omnipaste.Framework;
    using Omnipaste.UserToken;
    using System;
    using System.Windows;
    using System.Windows.Interop;
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.EventAggregatorMessages;
    using Configuration;
    using EventAggregatorMessages;
    using Omnipaste.NotificationList;
    using Omnipaste.Dialog;
    using Properties;
    using System.Collections.Generic;
    using System.Deployment.Application;
    using System.Reflection;
    using Clipboard;
    using Notifications;
    using Notifications.NotificationList;
    using OmniApi;
    using OmniCommon.Framework;
    using OmniCommon.Interfaces;
    using System.Linq;
    using Omnipaste.Framework;
    using Omnipaste.UserToken;

    public sealed class ShellViewModel : Conductor<IWorkspace>.Collection.OneActive, IShellViewModel
    {
        private Window _view;

        public IUserTokenViewModel UserToken { get; set; }

        [Inject]
        public IWindowManager WindowManager { get; set; }

        [Inject]
        public IKernel Kernel { get; set; }

        public IConfigurationViewModel ConfigurationViewModel { get; set; }

        public IEventAggregator EventAggregator { get; set; }

        public IApplicationWrapper ApplicationWrapper { get; set; }

        public string TooltipText { get; set; }

        public string IconSource { get; set; }

        public bool IsNotSyncing { get; set; }

        public Visibility Visibility { get; set; }

        [Inject]
        public IDialogViewModel DialogViewModel { get; set; }

        public ShellViewModel(IConfigurationViewModel configurationViewModel, IEventAggregator eventAggregator, IUserTokenViewModel userToken, IDialogService service)
        {
            service.Start();
            UserToken = userToken;

            EventAggregator = eventAggregator;
            EventAggregator.Subscribe(this);

            ConfigurationViewModel = configurationViewModel;

            DisplayName = Resources.AplicationName;
            ApplicationWrapper = new ApplicationWrapper();

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                var ad = ApplicationDeployment.CurrentDeployment;
                version = ad.CurrentVersion;
            }

            TooltipText = "Omnipaste " + version;
            IconSource = "/Icon.ico";
        }

        public void Handle(GetTokenFromUserMessage message)
        {
            UserToken.Message = message.Message;
            DialogViewModel.ActivateItem(UserToken);
        }

        public void Exit()
        {
            Visibility = Visibility.Collapsed;
            ApplicationWrapper.ShutDown();
        }

        public void Handle(ConfigurationCompletedMessage message)
        {
            HandleSuccessfulLogin();

            EventAggregator.PublishOnCurrentThread(new StartOmniServiceMessage());

            var wm = new WindowManager();
            wm.ShowWindow(
                Kernel.Get<INotificationListViewModel>(),
                null,
                new Dictionary<string, object>
                {
                    {"Height", SystemParameters.WorkArea.Height},
                    {"Width", SystemParameters.WorkArea.Width}

                });
        }

        public void Show()
        {
            _view.Visibility = Visibility.Visible;
            _view.ShowInTaskbar = true;
        }

        public void HandleSuccessfulLogin()
        {
            Kernel.Load(new ClipboardModule(), new DevicesModule(), new NotificationsModule());

            RunStartupTasks();

            var startables = Kernel.GetAll<IStartable>();

            var count = startables.Count();
        }

        public void ToggleSync()
        {
            if (IsNotSyncing)
            {
                EventAggregator.PublishOnCurrentThread(new StopOmniServiceMessage());
            }
            else
            {
                EventAggregator.PublishOnCurrentThread(new StartOmniServiceMessage());
            }
        }

        private void RunStartupTasks()
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