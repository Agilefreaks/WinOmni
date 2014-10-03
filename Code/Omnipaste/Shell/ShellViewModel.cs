﻿namespace Omnipaste.Shell
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Deployment.Application;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Interop;
    using Caliburn.Micro;
    using Events;
    using Events.Handlers;
    using Ninject;
    using OmniCommon;
    using OmniCommon.EventAggregatorMessages;
    using OmniCommon.Framework;
    using OmniCommon.Interfaces;
    using Omnipaste.DataProviders;
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
            if (_view != null)
            {
                _view.Show();
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

#if !DEBUG
            if (ApplicationDeploymentHelper.IsClickOnceApplication && !ApplicationDeployment.CurrentDeployment.IsFirstRun)
            {
                Close();
            }
#endif
        }

        private void MigrateAwayFromClickOnce()
        {
            Process process = null;
            var exitCode = -1;

            try
            {
                var arguments = string.Format(
                    "-installerUri \"{0}\" -applicationName \"{1}\"",
                    ConfigurationManager.AppSettings[ConfigurationProperties.UpdateSource],
                    ConfigurationManager.AppSettings[ConfigurationProperties.AppName]);

                var processStartInfo = new ProcessStartInfo
                                       {
                                           FileName = "ClickOnceTransition.exe",
                                           Arguments = arguments,
                                           CreateNoWindow = true,
                                           UseShellExecute =false
                                       };

                process = Process.Start(processStartInfo);
            }
                // ReSharper disable once EmptyGeneralCatchClause
            catch 
            {
            }

            if (process != null)
            {
                process.WaitForExit();
                exitCode = process.ExitCode;
            }

            if (exitCode != 0)
            {
                ContextMenuViewModel.ShowBaloon("Update failed", "We tried to update Omnipaste but something went wrong. Please reinstall the application at your convenience.");
            }
        }

        private void Configure()
        {
            DialogViewModel.ActivateItem(LoadingViewModel.Loading());

            ActivationService.Run()
                .SubscribeOn(Scheduler.Default)
                .ObserveOn(SchedulerProvider.Dispatcher)
                .Subscribe(OnActivationFinished, OnActivationFailed);
            
            UpdaterService.CheckForUpdatesPeriodically()
                .ObserveOn(NewThreadScheduler.Default)
                .Subscribe(_ => UpdaterService.ApplyUpdate());
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

                //if (ApplicationDeploymentHelper.IsClickOnceApplication)
                {
                    Task.Factory.StartNew(MigrateAwayFromClickOnce);
                }
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