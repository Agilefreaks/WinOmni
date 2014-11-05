namespace Omnipaste
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using BugFreak;
    using Caliburn.Micro;
    using Castle.Core.Internal;
    using Clipboard;
    using Events;
    using Ninject;
    using Omni;
    using OmniApi;
    using OmniCommon;
    using OmniCommon.DataProviders;
    using OmniCommon.Interfaces;
    using OmniDebug;
    using Omnipaste.Services;
    using Omnipaste.Shell;
    using OmniSync;
    using OmniUI;
    using OmniUI.Attributes;
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

            SetupViewLocator();

            _kernel.Load(
                new OmniApiModule(),
                new OmniSyncModule(),
                new OmniUiModule(),
                new OmniModule(),
                new ClipboardModule(),
                new EventsModule(),
                new OmnipasteModule());
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
            var allStartedServices = GetAllInstances(typeof(IStartable)).Cast<IStartable>();
            allStartedServices.ForEach(s => s.Stop());

            base.OnExit(sender, e);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);

            var argumentsDataProvider = _kernel.Get<IArgumentsDataProvider>();
            var viewSettings = argumentsDataProvider.Minimized ? _minimizedViewStartOptions : _normalViewStartOptions;

            var configurationService = _kernel.Get<IConfigurationService>();

            BugFreak.Hook(
                configurationService[ConfigurationProperties.BugFreakApiKey],
                configurationService[ConfigurationProperties.BugFreakToken],
                Application.Current);

            if (configurationService.DebugMode)
            {
                _kernel.Load(new DebugModule());
            }

            SetupApplicationVersionLogging();

            DisplayRootViewFor<ShellViewModel>(viewSettings);

            _kernel.Get<IConnectivitySupervisor>();
        }

        private void SetupApplicationVersionLogging()
        {
            var applicationService = _kernel.Get<IApplicationService>();
            GlobalConfig.AdditionalData.Add(new KeyValuePair<string, string>("Application Version", applicationService.Version.ToString()));
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[] { Assembly.GetExecutingAssembly() };
        }

        public static void SetupViewLocator()
        {
            ViewLocator.LocateForModelType = (modelType, displayLocation, context) =>
            {
                var useViewAttributes =
                    modelType.GetCustomAttributes(typeof(UseViewAttribute), true)
                        .Cast<UseViewAttribute>().ToArray();

                string viewTypeName;

                if (useViewAttributes.Count() == 1)
                {
                    var attribute = useViewAttributes.First();
                    viewTypeName = attribute.IsFullyQualifiedName
                                       ? attribute.ViewName
                                       : string.Concat(
                                           modelType.Namespace.Replace("Model",
                                                                       string.Empty), ".",
                                           attribute.ViewName);
                }
                else
                {
                    viewTypeName = modelType.FullName.Replace("Model", string.Empty);
                }

                if (context != null)
                {
                    viewTypeName = viewTypeName.Remove(viewTypeName.Length - 4, 4);
                    viewTypeName = viewTypeName + "." + context;
                }

                var viewType = (from assembly in AssemblySource.Instance
                                from type in assembly.GetExportedTypes()
                                where type.FullName == viewTypeName
                                select type).FirstOrDefault();

                return viewType == null
                           ? new TextBlock { Text = string.Format("{0} not found.", viewTypeName) }
                           : ViewLocator.GetOrCreateViewType(viewType);
            };
        }

        #endregion
    }
}