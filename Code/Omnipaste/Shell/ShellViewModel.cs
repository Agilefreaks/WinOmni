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
    using Omnipaste.ClippingList;
    using Omnipaste.Dialog;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Framework;
    using Omnipaste.Loading;
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

        #endregion

        #region Constructors and Destructors

        public ShellViewModel(IEventAggregator eventAggregator, ISessionManager sessionManager)
        {
            EventAggregator = eventAggregator;
            EventAggregator.Subscribe(this);

            sessionManager.SessionDestroyedObservable().Subscribe(eventArgs => Execute.OnUIThread(Configure));

            DisplayName = Resources.AplicationName;
        }

        #endregion

        #region Public Properties

        [Inject]
        public IActivationService ActivationService { get; set; }

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
                ContextMenuViewModel.ShowBaloon("Running in the background", "Omnipaste is still running. To open the window again, double-click the icon.");
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

#if !DEBUG
            if (ApplicationDeploymentHelper.IsClickOnceApplication && !ApplicationDeployment.CurrentDeployment.IsFirstRun)
            {
                Close();
            }
#endif
        }

        private void Configure()
        {
            DialogViewModel.ActivateItem(LoadingViewModel.Loading());

            ActivationService.Run()
                .SubscribeOn(Scheduler.Default)
                .ObserveOn(SchedulerProvider.Dispatcher)
                .Subscribe(
                    finalStep =>
                    {
                        if (finalStep is Failed)
                        {
                            EventAggregator.PublishOnUIThread(new ActivationFailedMessage { Exception = new Exception("Final step was Failed") });
                        }
                        else
                        {
                            ClippingListViewModel = Kernel.Get<IMasterClippingListViewModel>();

                            DialogViewModel.DeactivateItem(LoadingViewModel, true);
                            NotificationListViewModel.ShowWindow(
                                WindowManager,
                                Kernel.Get<INotificationListViewModel>());
                        }
                    },
                    exception =>
                    EventAggregator.PublishOnUIThread(new ActivationFailedMessage { Exception = exception }));
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