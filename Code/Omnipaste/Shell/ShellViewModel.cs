namespace Omnipaste.Shell
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Windows;
    using System.Windows.Interop;
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using Omnipaste.Dialog;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Loading;
    using Omnipaste.NotificationList;
    using Omnipaste.Properties;
    using Omnipaste.Services;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;
    using Omnipaste.Shell.Connection;
    using Omnipaste.Shell.ContextMenu;
    using Omnipaste.Workspaces;
    using OmniUI.Flyout;
    using OmniUI.HeaderButton;

    public sealed class ShellViewModel : Conductor<IScreen>.Collection.OneActive, IShellViewModel
    {
        #region Fields

        private Window _view;

        private int _selectedViewIndex;

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

        [Inject]
        public INotificationListViewModel NotificationListViewModel { get; set; }

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
        public IDialogViewModel DialogViewModel { get; set; }

        [Inject]
        public IEnumerable<IFlyoutViewModel> Flyouts { get; set; }

        [Inject]
        public ILoadingViewModel LoadingViewModel { get; set; }

        [Inject]
        public IEnumerable<IHeaderButtonViewModel> HeaderButtonViewModels { get; set; }

        [Inject]
        public IEnumerable<IHeaderItemViewModel> HeaderItemViewModels { get; set; }

        [Inject]
        public IWindowHandleProvider WindowHandleProvider { get; set; }

        [Inject]
        public IUiRefreshService UiRefreshService { get; set; }

        [Inject]
        public IEnumerable<IWorkspace> Workspaces { get; set; }

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
            UiRefreshService.Stop();
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
                    _view.Activate();
                });
            }

            UiRefreshService.Start();
        }

        #endregion

        #region Methods

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            ActivateItem(Workspaces.OfType<ActivityWorkspaceViewModel>().First());

            _view = (Window)view;
            _view.Closing += Closing;

            WindowHandleProvider.SetHandle(GetHandle());
            NotificationListViewModel.Show();

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
                DialogViewModel.DeactivateItem(LoadingViewModel, true);
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