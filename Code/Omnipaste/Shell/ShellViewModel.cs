namespace Omnipaste.Shell
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Interop;
    using Caliburn.Micro;
    using Clipboard.Models;
    using Ninject;
    using Omni;
    using OmniCommon.Framework;
    using Omnipaste.ClippingList;
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

        private Window _view;

        private IMasterClippingListViewModel _clippingListViewModel;

        #endregion

        #region Constructors and Destructors

        public ShellViewModel(IEventAggregator eventAggregator, ISessionManager sessionManager)
        {
            eventAggregator.Subscribe(this);
            sessionManager.SessionDestroyed += (sender, args) => Configure();

            DisplayName = Resources.AplicationName;
        }

        #endregion

        #region Public Properties

        [Inject]
        public IActivationService ActivationService { get; set; }

        [Inject]
        public ISettingsHeaderViewModel SettingsHeaderViewModel { get; set; }

        [Inject]
        public IEnumerable<IFlyoutViewModel> Flyouts { get; set; }

        [Inject]
        public IConnectionViewModel ConnectionViewModel { get; set; }

        [Inject]
        public IContextMenuViewModel ContextMenuViewModel { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        [Inject]
        public IDialogViewModel DialogViewModel { get; set; }

        [Inject]
        public IKernel Kernel { get; set; }

        [Inject]
        public ILoadingViewModel LoadingViewModel { get; set; }

        public IMasterClippingListViewModel ClippingListViewModel
        {
            get
            {
                return _clippingListViewModel;
            }
            set
            {
                _clippingListViewModel = value;
                NotifyOfPropertyChange(() => ClippingListViewModel);
            }
        }

        [Inject]
        public IOmniService OmniService { get; set; }

        [Inject]
        public IWindowManager WindowManager { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Close();
        }

        public void Close()
        {
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

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            _view = (Window)view;
            _view.Closing += Closing;

            Kernel.Bind<IntPtr>().ToMethod(context => GetHandle());
            Configure();
        }

        private void Configure()
        {
            DialogViewModel.ActivateItem(LoadingViewModel);
            ActivationService.Run()
                .SubscribeOn(Scheduler.Default)
                .Subscribe(
                    finalStep =>
                    {
                        OmniService.Start().Wait();

                        ClippingListViewModel = Kernel.Get<IMasterClippingListViewModel>();
                        Execute.OnUIThread(
                            () =>
                            {
                                DialogViewModel.DeactivateItem(LoadingViewModel, true);
                                NotificationListViewModel.ShowWindow(WindowManager, Kernel.Get<INotificationListViewModel>());
                            });
                    },
                exception => Debugger.Break()
                );
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