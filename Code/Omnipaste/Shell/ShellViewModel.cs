namespace Omnipaste.Shell
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Deployment.Application;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Windows;
    using System.Windows.Interop;
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.EventAggregatorMessages;
    using OmniCommon.Framework;
    using OmniCommon.Interfaces;
    using Omnipaste.MasterClippingList;
    using Omnipaste.Dialog;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Framework;
    using Omnipaste.Loading;
    using Omnipaste.MasterEventList;
    using Omnipaste.NotificationList;
    using Omnipaste.Properties;
    using Omnipaste.Services;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;
    using Omnipaste.Shell.Connection;
    using Omnipaste.Shell.ContextMenu;
    using Omnipaste.Shell.SettingsHeader;

    public sealed class ShellViewModel : Conductor<IWorkspace>.Collection.OneActive, IShellViewModel
    {
        #region Fields

        private IMasterClippingListViewModel _clippingListViewModel;

        private Window _view;

        private int _selectedViewIndex;

        private IMasterEventListViewModel _masterEventListViewModel;

        private INotificationListViewModel _notificationListViewModel;

        #endregion

        #region Constructors and Destructors

        public ShellViewModel(IEventAggregator eventAggregator, ISessionManager sessionManager)
        {
            EventAggregator = eventAggregator;
            EventAggregator.Subscribe(this);

            sessionManager.SessionDestroyedObservable()
                .ObserveOn(SchedulerProvider.Dispatcher)
                .Subscribe(eventArgs => Configure());

            DisplayName = Resources.AplicationName;
        }

        #endregion

        #region Public Properties

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

        public IMasterEventListViewModel MasterEventListViewModel
        {
            get
            {
                return _masterEventListViewModel;
            }
            set
            {
                _masterEventListViewModel = value;
                NotifyOfPropertyChange(() => MasterEventListViewModel);
            }
        }

        public int SelectedViewIndex
        {
            get
            {
                return _selectedViewIndex;
            }
            set
            {
                _selectedViewIndex = value;
                NotifyOfPropertyChange(() => SelectedViewIndex);
            }
        }

        [Inject]
        public IActivationService ActivationService { get; set; }

        [Inject]
        public IConnectionViewModel ConnectionViewModel { get; set; }

        [Inject]
        public IContextMenuViewModel ContextMenuViewModel { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        [Inject]
        public IDialogViewModel DialogViewModel { get; set; }

        public IEventAggregator EventAggregator { get; set; }

        [Inject]
        public IEnumerable<IFlyoutViewModel> Flyouts { get; set; }

        [Inject]
        public IKernel Kernel { get; set; }

        [Inject]
        public ILoadingViewModel LoadingViewModel { get; set; }

        [Inject]
        public ISettingsHeaderViewModel SettingsHeaderViewModel { get; set; }

        [Inject]
        public IWindowManager WindowManager { get; set; }

        [Inject]
        public IUpdaterService UpdaterService { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Close()
        {
            if (_view != null)
            {
                _view.Hide();
            }

            if (ApplicationDeploymentHelper.IsClickOnceApplication && ApplicationDeployment.CurrentDeployment.IsFirstRun)
            {
                ContextMenuViewModel.ShowBaloon(Resources.ShellBallonTitle, Resources.ShellBallonContent);
            }
        }

        public void Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Close();
        }

        public void Handle(ShowShellMessage message)
        {
            Show();
        }

        public void Handle(RetryMessage message)
        {
            Configure();
        }

        public void Show()
        {
            if (_view != null)
            {
                _view.Dispatcher.Invoke(() =>
                {
                    _view.Show();
                    _view.ShowInTaskbar = true;
                    _view.Visibility = Visibility.Visible;
                });
            }
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
            DialogViewModel.ActivateItem(LoadingViewModel.Loading());

            ActivationService.Run()
                .SubscribeOn(Scheduler.Default)
                .ObserveOn(SchedulerProvider.Dispatcher)
                .Subscribe(OnActivationFinished, OnActivationFailed);
        }

        private void OnActivationFailed(Exception exception)
        {
            EventAggregator.PublishOnUIThread(new ActivationFailedMessage { Exception = exception });
        }

        private void OnActivationFinished(IActivationStep finalStep)
        {
            if (finalStep is Failed)
            {
                EventAggregator.PublishOnUIThread(new ActivationFailedMessage { Exception = finalStep.Parameter.Value as Exception });
            }
            else
            {
                ClippingListViewModel = Kernel.Get<IMasterClippingListViewModel>();
                MasterEventListViewModel = Kernel.Get<IMasterEventListViewModel>();
#if !DEBUG

                Close();
#endif

                DialogViewModel.DeactivateItem(LoadingViewModel, true);
                _notificationListViewModel = Kernel.Get<INotificationListViewModel>();
                NotificationListViewModel.ShowWindow(WindowManager, _notificationListViewModel);
            }
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