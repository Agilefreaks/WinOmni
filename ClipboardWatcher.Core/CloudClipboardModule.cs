﻿using System.Linq;
using System.Reflection;
using ClipboardWatcher.Core.Impl.PubNub;
using ClipboardWatcher.Core.Services;
using Ninject;
using Ninject.Modules;

namespace ClipboardWatcher.Core
{
    public class CloudClipboardModule : NinjectModule
    {
        public override void Load()
        {
            // The configuration service will be resolved by both its type (because it implements the StartupTask) 
            // and the interface's type - as it was designed to be obtained
            Kernel.Bind<ConfigurationService>().ToSelf().InSingletonScope();
            Kernel.Bind<IConfigurationService>().ToMethod(c => Kernel.Get<ConfigurationService>());

            Kernel.Bind<IPubNubClientFactory>().To<PubNubClientFactory>().InSingletonScope();
            Kernel.Bind<IApplicationDeploymentInfo>().To<ApplicationDeploymentWrapper>().InSingletonScope();

            PerfornStartupTasks();
        }

        private void PerfornStartupTasks()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var startupTaskType = typeof(IStartupTask);
            var typesToIgnore = new[] { startupTaskType };
            executingAssembly.GetExportedTypes()
                             .Where(startupTaskType.IsAssignableFrom)
                             .Except(typesToIgnore)
                             .ToList()
                             .ForEach(type => ((IStartupTask)Kernel.Get(type)).Startup());
        }
    }
}