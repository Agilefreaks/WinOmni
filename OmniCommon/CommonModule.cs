using System.Linq;
using System.Reflection;
using Ninject;
using Ninject.Modules;
using OmniCommon.Interfaces;
using OmniCommon.Services;

namespace OmniCommon
{
    public class CommonModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IOmniService>().To<OmniService>().InSingletonScope();
            // The configuration service will be resolved by both its type (because it implements the StartupTask) 
            // and the interface's type - as it was designed to be obtained
            Kernel.Bind<ConfigurationService>().ToSelf().InSingletonScope();
            Kernel.Bind<IConfigurationService>().ToMethod(c => Kernel.Get<ConfigurationService>());
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