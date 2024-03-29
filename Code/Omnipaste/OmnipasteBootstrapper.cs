﻿namespace Omnipaste
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows;
    using BugFreak;
    using Caliburn.Micro;
    using Clipboard;
    using CommonServiceLocator.NinjectAdapter.Unofficial;
    using Contacts;
    using Humanizer.Configuration;
    using Humanizer.DateTimeHumanizeStrategy;
    using Microsoft.Practices.ServiceLocation;
    using Ninject;
    using Omni;
    using OmniApi;
    using OmniCommon;
    using OmniCommon.DataProviders;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using OmniDebug;
    using Omnipaste.Framework.Helpers;
    using Omnipaste.Framework.Services;
    using Omnipaste.Shell;
    using OmniSync;
    using OmniUI;
    using PhoneCalls;
    using SMS;

    public class OmnipasteBootstrapper : BootstrapperBase
    {
        #region Fields

        private readonly Dictionary<string, object> _normalViewStartOptions = new Dictionary<string, object>
            {
                { "ShowInTaskbar", true },
                { "Visibility", Visibility.Visible }
            };

        private readonly Dictionary<string, object> _minimizedViewStartOptions = new Dictionary<string, object>
            {
                { "ShowInTaskbar", false },
                { "Visibility", Visibility.Hidden }
            };

        private IKernel _kernel;

        #endregion

        #region Constructors and Destructors

        public OmnipasteBootstrapper()
        {
            Initialize();
        }

        #endregion

        #region Methods

        protected override void BuildUp(object instance)
        {
            _kernel.Inject(instance);
        }

        protected override void Configure()
        {
            _kernel = new StandardKernel();
            _kernel.Load(
                new OmniApiModule(),
                new OmniSyncModule(),
                new OmniUiModule(),
                new OmniModule(),
                new ClipboardModule(),
                new ContactsModule(),
                new SMSModule(),
                new PhoneCallsModule(),
                new OmnipasteModule());
            ViewLocator.LocateForModelType = Framework.ViewLocator.LocateForModelType;
            
            var locator = new NinjectServiceLocator(_kernel);
            _kernel.Bind<IServiceLocator>().ToConstant(locator);
            ServiceLocator.SetLocatorProvider(() => locator);
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            return _kernel.Get(serviceType);
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            ApplicationHelper.Instance.StopBackgroundProcesses();

            base.OnExit(sender, e);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DispatcherProvider.Instance = new WpfDispatcherProvider();

            base.OnStartup(sender, e);

            var argumentsDataProvider = _kernel.Get<IArgumentsDataProvider>();
            var viewSettings = argumentsDataProvider.Minimized ? _minimizedViewStartOptions : _normalViewStartOptions;

            var configurationService = _kernel.Get<IConfigurationService>();

            BugFreak.Hook(
                configurationService[ConfigurationProperties.BugFreakApiKey],
                configurationService[ConfigurationProperties.BugFreakToken],
                Application.Current);
            ExceptionReporter.Instance = _kernel.Get<IExceptionReporter>();

            GlobalExceptionLogger.Hook();
            
            Configurator.DateTimeHumanizeStrategy = new PrecisionDateTimeHumanizeStrategy();

            if (configurationService.DebugMode)
            {
                _kernel.Load(new DebugModule());
            }

            SimpleLogger.EnableLog = argumentsDataProvider.EnableLog || configurationService.LoggingEnabled;

            SetupBugFreakAdditionalData();
            StartBackgroundServices();

            DisplayRootViewFor<IShellViewModel>(viewSettings);
        }

        private void SetupBugFreakAdditionalData()
        {
            var configurationService = _kernel.Get<IConfigurationService>();
            GlobalConfig.AdditionalData.Add(new KeyValuePair<string, string>("Application Version", configurationService.Version.ToString()));
            GlobalConfig.AdditionalData.Add(new KeyValuePair<string, string>("Device Identifier", configurationService.DeviceIdentifier));
            GlobalConfig.AdditionalData.Add(new KeyValuePair<string, string>("Device Id", configurationService.DeviceId));
        }

        private void StartBackgroundServices()
        {
            ApplicationHelper.Instance.StartBackgroundService<IConnectivitySupervisor>();
            ApplicationHelper.Instance.StartBackgroundService<IUpdaterService>();
            ApplicationHelper.Instance.StartBackgroundService<IEntitySupervisor>();
        }

        #endregion
    }
}