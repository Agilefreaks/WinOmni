namespace Omnipaste.Shell
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Threading;
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.EventAggregatorMessages;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
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
    using OmniUI.Flyout;
    using OmniUI.HeaderButton;

    public sealed class ShellViewModel : Conductor<IScreen>.Collection.OneActive, IShellViewModel
    {
        #region Fields

        private IMasterClippingListViewModel _clippingListViewModel;

        private Window _view;

        private int _selectedViewIndex;

        private IMasterEventListViewModel _masterEventListViewModel;

        private INotificationListViewModel _notificationListViewModel;

        private readonly IDisposable _sessionObserver;

        #endregion

        #region Constructors and Destructors

        public ShellViewModel(IEventAggregator eventAggregator, ISessionManager sessionManager)
        {
            EventAggregator = eventAggregator;
            EventAggregator.Subscribe(this);

            _sessionObserver = sessionManager.SessionDestroyedObservable
                .ObserveOn(SchedulerProvider.Dispatcher)
                .SubscribeAndHandleErrors(eventArgs => Configure());

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

        public IEventAggregator EventAggregator { get; set; }

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

        [Inject]
        public IEnumerable<IFlyoutViewModel> Flyouts { get; set; }

        [Inject]
        public IKernel Kernel { get; set; }

        [Inject]
        public ILoadingViewModel LoadingViewModel { get; set; }

        [Inject]
        public IWindowManager WindowManager { get; set; }

        [Inject]
        public IUpdaterService UpdaterService { get; set; }

        [Inject]
        public IEnumerable<IHeaderButtonViewModel> HeaderButtonViewModels { get; set; }

        [Inject]
        public IEnumerable<IHeaderItemViewModel> HeaderItemViewModels { get; set; }

        [Inject]
        public IWindowHandleProvider WindowHandleProvider { get; set; }

        public IEnumerable<IHeaderItemViewModel> AllHeaderItems
        {
            get
            {
                return HeaderItemViewModels.Concat(HeaderButtonViewModels);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Close()
        {
            if (_view != null)
            {
                _view.Hide();
            }

            ContextMenuViewModel.ShowBalloon(Resources.ShellBallonTitle, Resources.ShellBallonContent);
        }

        public void Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Close();
        }

        public void Dispose()
        {
            _sessionObserver.Dispose();
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

            WindowHandleProvider.SetHandle(GetHandle());

            Configure();
        }

        private void Configure()
        {
            DialogViewModel.ActivateItem(LoadingViewModel.Loading());

            ActivationService.Run()
                .RunToCompletion(OnActivationFinished, OnActivationFailed, DispatcherProvider.Current);
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