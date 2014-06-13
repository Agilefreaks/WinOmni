﻿namespace Omnipaste
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using Caliburn.Micro;
    using Castle.Core.Internal;
    using Clipboard;
    using Ninject;
    using Ninject.Extensions.Conventions;
    using Notifications;
    using Omni;
    using OmniApi;
    using OmniCommon;
    using OmniCommon.Interfaces;
    using Omnipaste.Dialog;
    using Omnipaste.Framework;
    using Omnipaste.Services.Connectivity;
    using Omnipaste.Shell;
    using OmniSync;

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
            var singletonViewModelTypes = new List<Type> { typeof(ShellViewModel), typeof(DialogViewModel) };
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

            _kernel.Bind(
                configure =>
                configure.FromThisAssembly()
                    .Select(t => t.Name.EndsWith("Service"))
                    .BindDefaultInterface()
                    .Configure(c => c.InSingletonScope()));

            _kernel.Bind(x => x.FromThisAssembly().Select(t => t.Name.EndsWith("StartupTask")).BindAllInterfaces());
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

        protected void RunStartupTasks()
        {
            foreach (var task in _kernel.GetAll<IStartupTask>())
            {
                task.Startup();
            }
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[] { Assembly.GetExecutingAssembly() };
        }

        #endregion
    }
}