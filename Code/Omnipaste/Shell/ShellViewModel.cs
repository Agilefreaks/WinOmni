namespace Omnipaste.Shell
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
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
    using Omnipaste.Shell.ContextMenu;
    using Omnipaste.Shell.SideMenu;
    using Omnipaste.Workspaces.Activity;
    using OmniUI.Flyout;
    using OmniUI.TitleBarItem;

    public sealed class ShellViewModel : Conductor<IScreen>.Collection.OneActive, IShellViewModel
    {
        #region Fields

        private readonly IDisposable _sessionObserver;

        private Window _view;

        #endregion

        #region Constructors and Destructors

        public ShellViewModel(IEventAggregator eventAggregator, ISessionManager sessionManager)
        {
            EventAggregator = eventAggregator;
            EventAggregator.Subscribe(this);

            _sessionObserver =
                sessionManager.SessionDestroyedObservable.ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeAndHandleErrors(eventArgs => Configure());

            DisplayName = Resources.AplicationName;
        }

        #endregion

        #region Public Properties

        [Inject]
        public IActivationService ActivationService { get; set; }

        [Inject]
        public IContextMenuViewModel ContextMenuViewModel { get; set; }

        [Inject]
        public IActivityWorkspace DefaultWorkspace { get; set; }

        [Inject]
        public IDialogViewModel DialogViewModel { get; set; }

        public IEventAggregator EventAggregator { get; set; }

        [Inject]
        public IEnumerable<IFlyoutViewModel> Flyouts { get; set; }

        [Inject]
        public ILoadingViewModel LoadingViewModel { get; set; }

        [Inject]
        public INotificationListViewModel NotificationListViewModel { get; set; }

        [Inject]
        public ISideMenuViewModel SideMenuViewModel { get; set; }

        [Inject]
        public IEnumerable<ITitleBarItemViewModel> TitleBarItems { get; set; }

        [Inject]
        public IUiRefreshService UiRefreshService { get; set; }

        [Inject]
        public IWindowHandleProvider WindowHandleProvider { get; set; }

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
            SideMenuViewModel.Dispose();
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
                _view.Dispatcher.Invoke(
                    () =>
                        {
                            _view.Show();
                            _view.ShowInTaskbar = true;
                            _view.Visibility = Visibility.Visible;
                            _view.WindowState = WindowState.Normal;
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
            ActivateItem(DefaultWorkspace);

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

        private void OnActivationFailed(Exception exception)
        {
            EventAggregator.PublishOnUIThread(new ActivationFailedMessage { Exception = exception });
        }

        private void OnActivationFinished(IActivationStep finalStep)
        {
            if (finalStep is Failed)
            {
                EventAggregator.PublishOnUIThread(
                    new ActivationFailedMessage { Exception = finalStep.Parameter.Value as Exception });
            }
            else
            {
                DialogViewModel.DeactivateItem(LoadingViewModel, true);
            }
        }

        #endregion
    }
}