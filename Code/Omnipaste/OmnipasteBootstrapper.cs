namespace Omnipaste
{
    using System.Linq;
    using System.Windows;
    using Castle.Core.Internal;
    using OmniApi;
    using OmniCommon.Interfaces;
    using Omnipaste.Framework;
    using Omnipaste.Services.Connectivity;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Caliburn.Micro;
    using Ninject;
    using Ninject.Extensions.Conventions;
    using Omni;
    using Omnipaste.NotificationList;
    using OmniSync;
    using OmniCommon;
    using Omnipaste.Shell;

    public class OmnipasteBootstrapper : BootstrapperBase
    {
        private IKernel _kernel;

        public OmnipasteBootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);

            DisplayRootViewFor<ShellViewModel>();
        }


        protected override void Configure()
        {
            var singletonViewModelTypes = new List<Type> { typeof(ShellViewModel) };
            _kernel = new StandardKernel();

            _kernel.Load(
                new OmniCommonModule(),
                new OmniApiModule(),
                new OmniSyncModule(),
                new OmniModule(),
                new OmnipasteModule());

            _kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
            _kernel.Bind<IOmniServiceHandler>().To<OmniServiceHandler>().InSingletonScope();
            _kernel.Bind<IConnectivityHelper>().To<ConnectivityHelper>().InSingletonScope();

            _kernel.Bind(
                configure =>
                configure.FromThisAssembly()
                 .Select(singletonViewModelTypes.Contains)
                 .BindDefaultInterface()
                 .Configure(c => c.InSingletonScope()));
            _kernel.Bind(
                configure =>
                configure.FromThisAssembly()
                 .Select(t => t.Name.EndsWith("ViewModel") && !singletonViewModelTypes.Contains(t))
                 .BindDefaultInterface());
            _kernel.Bind(x => x.FromThisAssembly().Select(t => t.Name.EndsWith("StartupTask")).BindAllInterfaces());

            _kernel.Bind<IConnectivityNotifyService>().ToConstant(CreateConnectivityService());
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            var allStartedServices = GetAllInstances(typeof(IStartable)).Cast<IStartable>();
            allStartedServices.ForEach(s => s.Stop());

            base.OnExit(sender, e);
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            return _kernel.Get(serviceType);
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }

        protected override void BuildUp(object instance)
        {
            _kernel.Inject(instance);
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[]
                       {
                           Assembly.GetExecutingAssembly()
                       };
        }

        protected void RunStartupTasks()
        {
            foreach (var task in _kernel.GetAll<IStartupTask>())
            {
                task.Startup();
            }
        }

        protected IConnectivityNotifyService CreateConnectivityService()
        {
            var connectivityObserver = new ConnectivityNotifyService();
            Application.Exit += (sender, args) => connectivityObserver.Stop();
            connectivityObserver.Start();

            return connectivityObserver;
        }
    }
}