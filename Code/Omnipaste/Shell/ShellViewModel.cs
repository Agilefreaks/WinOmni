namespace Omnipaste.Shell
{
    using System;
    using System.Collections.Generic;
    using System.Deployment.Application;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Windows;
    using System.Windows.Interop;
    using Caliburn.Micro;
    using Clipboard;
    using Ninject;
    using Notifications;
    using OmniApi;
    using OmniCommon.EventAggregatorMessages;
    using OmniCommon.Framework;
    using Omnipaste.Configuration;
    using Omnipaste.Dialog;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Framework;
    using Omnipaste.Loading;
    using Omnipaste.NotificationList;
    using Omnipaste.Properties;
    using Omnipaste.UserToken;

    public sealed class ShellViewModel : Conductor<IWorkspace>.Collection.OneActive, IShellViewModel
    {
        #region Fields

        private Window _view;

        #endregion

        #region Constructors and Destructors

        public ShellViewModel(
            IConfigurationViewModel configurationViewModel,
            IEventAggregator eventAggregator,
            IUserTokenViewModel userToken,
            IDialogService service)
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

        #endregion

        #region Public Properties

        public IApplicationWrapper ApplicationWrapper { get; set; }

        public IConfigurationViewModel ConfigurationViewModel { get; set; }

        [Inject]
        public IDialogViewModel DialogViewModel { get; set; }

        public IEventAggregator EventAggregator { get; set; }

        public string IconSource { get; set; }

        public bool IsNotSyncing { get; set; }

        [Inject]
        public IKernel Kernel { get; set; }

        [Inject]
        public ILoadingViewModel LoadingViewModel { get; set; }

        public string TooltipText { get; set; }

        public IUserTokenViewModel UserToken { get; set; }

        public Visibility Visibility { get; set; }

        [Inject]
        public IWindowManager WindowManager { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Exit()
        {
            Visibility = Visibility.Collapsed;
            ApplicationWrapper.ShutDown();
        }

        public void Handle(GetTokenFromUserMessage message)
        {
            UserToken.Message = message.Message;
            //DialogViewModel.ActivateItem(UserToken);
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
                        { "Height", SystemParameters.WorkArea.Height },
                        { "Width", SystemParameters.WorkArea.Width }
                    });
        }

        public void HandleSuccessfulLogin()
        {
            Kernel.Load(new ClipboardModule(), new DevicesModule(), new NotificationsModule());
            Kernel.Get<IOmniServiceHandler>().Init();

            var startables = Kernel.GetAll<IStartable>();

            var count = startables.Count();
        }

        public void Show()
        {
            _view.Visibility = Visibility.Visible;
            _view.ShowInTaskbar = true;
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

        #endregion

        #region Methods

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            _view = (Window)view;

            Kernel.Bind<IntPtr>().ToMethod(context => GetHandle());

            DialogViewModel.ActivateItem(LoadingViewModel);

            Thread.Sleep(200);

            ActiveItem = ConfigurationViewModel;
            ConfigurationViewModel.Start().ContinueWith(t => { });
        }

        private IntPtr GetHandle()
        {
            var handle = new IntPtr();
            Execute.OnUIThread(
                () =>
                    {
                        var windowInteropHelper = new WindowInteropHelper(_view);
                        handle = windowInteropHelper.Handle;
                    });

            return handle;
        }

        #endregion
    }
}