using System.Linq;
using System.Reflection;
using ClipboardWatcher.Core.Impl.PubNub;
using ClipboardWatcher.Core.Services;
using Ninject;
using Ninject.Modules;

namespace ClipboardWatcher.Core
{
    public class ClipboardWatcherCoreModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<ICloudClipboard>().To<PubNubCloudClipboard>().InSingletonScope();
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