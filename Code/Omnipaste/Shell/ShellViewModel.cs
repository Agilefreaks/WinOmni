namespace Omnipaste.Shell
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Interop;
    using Caliburn.Micro;
    using Clipboard;
    using Ninject;
    using Notifications;
    using OmniCommon.EventAggregatorMessages;
    using OmniCommon.Framework;
    using Omnipaste.Configuration;
    using Omnipaste.Connection;
    using Omnipaste.Dialog;
    using Omnipaste.Framework;
    using Omnipaste.Loading;
    using Omnipaste.NotificationList;
    using Omnipaste.Properties;
    using Omnipaste.Shell.ContextMenu;
    using Omnipaste.UserToken;

    public sealed class ShellViewModel : Conductor<IWorkspace>.Collection.OneActive, IShellViewModel
    {
        #region Fields

        private Window _view;

        private IConnectionViewModel _connectionViewModel;

        private IConnectionViewModel _connectionViewModel1;

        #endregion

        #region Constructors and Destructors

        public ShellViewModel(
            IConfigurationViewModel configurationViewModel,
            IUserTokenViewModel userToken,
            IContextMenuViewModel contextMenuViewModel)
        {
            UserToken = userToken;

            ConfigurationViewModel = configurationViewModel;

            DisplayName = Resources.AplicationName;
            try
            {
                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    var ad = ApplicationDeployment.CurrentDeployment;
                    version = ad.CurrentVersion;
                }
            }
            catch (InvalidDeploymentException)
            {
            }

            ContextMenu = contextMenuViewModel;
            ContextMenu.ShellViewModel = this;
        }

        #endregion

        #region Public Properties

        public IConfigurationViewModel ConfigurationViewModel { get; set; }

        public IContextMenuViewModel ContextMenu { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        [Inject]
        public IDialogViewModel DialogViewModel { get; set; }

        public IConnectionViewModel ConnectionViewModel
        {
            get
            {
                return _connectionViewModel1;
            }
            set
            {
                _connectionViewModel1 = value;
                NotifyOfPropertyChange(() => ConnectionViewModel);
            }
        }

        public IEventAggregator EventAggregator { get; set; }

        [Inject]
        public IKernel Kernel { get; set; }

        [Inject]
        public ILoadingViewModel LoadingViewModel { get; set; }
        
        public IUserTokenViewModel UserToken { get; set; }

        [Inject]
        public IWindowManager WindowManager { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            _view.Hide();
        }

        

        public void Show()
        {
            _view.Show();
        }

        #endregion

        #region Methods

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            _view = (Window)view;
            _view.Closing += Closing;

            Kernel.Bind<IntPtr>().ToMethod(context => GetHandle());
            Configure().ContinueWith(a => { });
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

        private async Task Configure()
        {
            DialogViewModel.ActivateItem(LoadingViewModel);

            await ConfigurationViewModel.Start();
            HandleSuccessfulLogin();

            ConnectionViewModel = Kernel.Get<IConnectionViewModel>();
            await ConnectionViewModel.Connect();

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

        private void HandleSuccessfulLogin()
        {
            Kernel.Load(new ClipboardModule(), new NotificationsModule());
            var startables = Kernel.GetAll<IStartable>();
            var count = startables.Count();
        }

        #endregion
    }
}