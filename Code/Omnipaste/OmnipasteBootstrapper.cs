namespace Omnipaste
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using BugFreak;
    using Caliburn.Micro;
    using Castle.Core.Internal;
    using Clipboard;
    using CommonServiceLocator.NinjectAdapter.Unofficial;
    using Contacts;
    using Events;
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
    using Omnipaste.Services;
    using Omnipaste.Shell;
    using OmniSync;
    using OmniUI;
    using SMS;
    using ViewLocator = Caliburn.Micro.ViewLocator;

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

        private readonly List<IStartable> _backgroundServices = new List<IStartable>();

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
                new EventsModule(),
                new ContactsModule(),
                new SMSModule(),
                new OmnipasteModule());
            ViewLocator.LocateForModelType = Framework.ViewLocator.LocateForModelType;
            
            var locator = new NinjectServiceLocator(_kernel);
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
            var allStartedServices = GetAllInstances(typeof(IStartable)).Cast<IStartable>()
                .Concat(_backgroundServices).Distinct();
            allStartedServices.ForEach(s => s.Stop());
            _kernel.Get<IOmniService>().Dispose();

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

            Humanizer.Configuration.Configurator.DateTimeHumanizeStrategy = new PrecisionDateTimeHumanizeStrategy();

            if (configurationService.DebugMode)
            {
                _kernel.Load(new DebugModule());
            }

            SimpleLogger.EnableLog = argumentsDataProvider.EnableLog;

            SetupBugFreakAdditionalData();
            StartBackgroundServices();

            DisplayRootViewFor<IShellViewModel>(viewSettings);
        }

        private void SetupBugFreakAdditionalData()
        {
            var configurationService = _kernel.Get<IConfigurationService>();
            GlobalConfig.AdditionalData.Add(new KeyValuePair<string, string>("Application Version", configurationService.Version.ToString()));
            GlobalConfig.AdditionalData.Add(new KeyValuePair<string, string>("Device Identifier", configurationService.DeviceIdentifier));
        }

        private void StartBackgroundServices()
        {
            //Since the service implements IStartable it will be started as soon as it's activated
            _backgroundServices.Add(_kernel.Get<IConnectivitySupervisor>());
            _backgroundServices.Add(_kernel.Get<IUpdaterService>());
            _backgroundServices.Add(_kernel.Get<IEntitySupervisor>());
        }

        #endregion
    }
}