﻿namespace Omnipaste
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Caliburn.Micro;
    using Ninject;
    using Ninject.Extensions.Conventions;
    using OmniCommon;
    using Omnipaste.Shell;
    using PubNubClipboard;
    using WindowsClipboard;

    public class OmnipasteBootstrapper : Bootstrapper<IShellViewModel>
    {
        private IKernel _kernel;

        protected override void Configure()
        {
            var singletonViewModelTypes = new List<Type>
                                              {
                                                  typeof(ShellViewModel)
                                              };
            _kernel = new StandardKernel();

            _kernel.Load<OmniCommonModule>();
            _kernel.Load<OmnipasteModule>();
            _kernel.Load<WindowsClipboardModule>();
            _kernel.Load<PubNubClipboardModule>();

            _kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
            _kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();

            _kernel.Bind(x => x.FromThisAssembly().Select(singletonViewModelTypes.Contains).BindDefaultInterface().Configure(c => c.InSingletonScope()));
            _kernel.Bind(x => x.FromThisAssembly().Select(t => t.Name.EndsWith("ViewModel") && !singletonViewModelTypes.Contains(t)).BindDefaultInterface());
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
    }
}