namespace Omnipaste
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using Caliburn.Micro;
    using Castle.Core.Internal;
    using Ninject;
    using Ninject.Extensions.Conventions;
    using Omni;
    using OmniApi;
    using OmniCommon;
    using OmniCommon.Interfaces;
    using Omnipaste.Dialog;
    using Omnipaste.Framework;
    using Omnipaste.Framework.Attributes;
    using Omnipaste.NotificationList;
    using Omnipaste.Services;
    using Omnipaste.Shell;
    using Omnipaste.Shell.Settings;
    using OmniSync;
    using ViewLocator = Caliburn.Micro.ViewLocator;

    public class OmnipasteBootstrapper : BootstrapperBase
    {
        #region Fields

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
            var singletonViewModelTypes = new List<Type> { typeof(ShellViewModel), typeof(DialogViewModel), typeof(SettingsViewModel), typeof(NotificationViewModelFactory) };
            _kernel = new StandardKernel();

            SetupViewLocator();

            _kernel.Load(
                new OmniCommonModule(),
                new OmniApiModule(),
                new OmniSyncModule(),
                new OmniModule(),
                new OmnipasteModule());

            _kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
            _kernel.Bind<ISessionManager>().To<SessionManager>().InSingletonScope();
            
            _kernel.Bind(
                configure =>
                    configure.FromThisAssembly()
                        .Select(t => t.Name.EndsWith("Service"))
                        .BindDefaultInterface()
                        .Configure(c => c.InSingletonScope()));

            _kernel.Bind(
                configure =>
                configure.FromThisAssembly()
                    .Select(singletonViewModelTypes.Contains)
                    .BindDefaultInterface()
                    .Configure(c => c.InSingletonScope()));

            _kernel.Bind<IFlyoutViewModel>().ToConstant(_kernel.Get<ISettingsViewModel>());

            _kernel.Bind(
                configure =>
                configure.FromThisAssembly()
                    .Select(t => t.Name.EndsWith("ViewModel") && !singletonViewModelTypes.Contains(t))
                    .BindDefaultInterface());
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

            DisplayRootViewFor<ShellViewModel>();
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