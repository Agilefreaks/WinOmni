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
    using Ninject;
    using Omni;
    using OmniCommon.Framework;
    using Omnipaste.Dialog;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Framework;
    using Omnipaste.Loading;
    using Omnipaste.NotificationList;
    using Omnipaste.Properties;
    using Omnipaste.Services;
    using Omnipaste.Shell.Connection;
    using Omnipaste.Shell.ContextMenu;
    using Omnipaste.Shell.SettingsHeader;

    public sealed class ShellViewModel : Conductor<IWorkspace>.Collection.OneActive, IShellViewModel
    {
        #region Fields

        private IConnectionViewModel _connectionViewModel;

        private Window _view;

        private IEnumerable<IFlyoutViewModel> _flyouts;

        #endregion

        #region Constructors and Destructors

        public ShellViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.Subscribe(this);

            DisplayName = Resources.AplicationName;
        }

        #endregion

        #region Public Properties

        [Inject]
        public IActivationService ActivationService { get; set; }

        [Inject]
        public ISettingsHeaderViewModel SettingsHeaderViewModel { get; set; }

        [Inject]
        public IEnumerable<IFlyoutViewModel> Flyouts
        {
            get
            {
                return _flyouts;
            }
            set
            {
                _flyouts = value;
                NotifyOfPropertyChange(() => Flyouts);
            }
        }

        [Inject]
        public IConnectionViewModel ConnectionViewModel
        {
            get
            {
                return _connectionViewModel;
            }
            set
            {
                _connectionViewModel = value;
                NotifyOfPropertyChange(() => ConnectionViewModel);
            }
        }

        [Inject]
        public IContextMenuViewModel ContextMenuViewModel { get; set; }
        
        [Inject]
        public IDialogViewModel DialogViewModel { get; set; }

        [Inject]
        public IKernel Kernel { get; set; }

        [Inject]
        public ILoadingViewModel LoadingViewModel { get; set; }

        [Inject]
        public IOmniService OmniService { get; set; }

        [Inject]
        public IWindowManager WindowManager { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            _view.Hide();
        }

        public void Handle(ShowShellMessage message)
        {
            Show();
        }

        public void Show()
        {
            _view.Show();
        }

        #endregion

        #region Methods

        protected async override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            _view = (Window)view;
            _view.Closing += Closing;

            Kernel.Bind<IntPtr>().ToMethod(context => GetHandle());
            await Configure();
        }

        private async Task Configure()
        {
            DialogViewModel.ActivateItem(LoadingViewModel);

            await ActivationService.Run();

            await OmniService.Start();

            DialogViewModel.DeactivateItem(LoadingViewModel, true);

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