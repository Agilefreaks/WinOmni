namespace Omnipaste
{
    using System.Linq;
    using System.Reflection;
    using Ninject;
    using Ninject.Modules;
    using OmniCommon.Interfaces;
    using Omnipaste.Services;
    using WindowsClipboard.Interfaces;

    public class MainModule : NinjectModule
    {
#if DEBUG
        public const string ApplicationName = "Omnipaste-Debug";
#elif STAGING
        public const string ApplicationName = "Omnipaste-Staging";
#else
        public const string ApplicationName = "Omnipaste";
#endif

        public override void Load()
        {
            Kernel.Bind<MainForm>().ToSelf().InSingletonScope();
            Kernel.Bind<ConfigureForm>().ToSelf().InSingletonScope();
            Kernel.Bind<IConfigureDialog>().ToMethod(c => c.Kernel.Get<ConfigureForm>());
            Kernel.Bind<IDelegateClipboardMessageHandling>().ToMethod(c => c.Kernel.Get<MainForm>());
            Kernel.Bind<IConfigurationProvider>().To<DPAPIConfigurationProvider>().InSingletonScope();
            Kernel.Bind<ConfigurationService>().ToSelf().InSingletonScope();
            Kernel.Bind<IConfigurationService>().ToMethod(c => Kernel.Get<ConfigurationService>());
#if DEBUG
            Kernel.Bind<IActivationDataProvider>().To<MockActivationDataProvider>().InSingletonScope();
            Kernel.Bind<IApplicationDeploymentInfoProvider>().To<MockApplicationDeploymentInfoProvider>();
#else
            Kernel.Bind<IActivationDataProvider>().To<ClickOnceActivationDataProvider>().InSingletonScope();
            Kernel.Bind<IApplicationDeploymentInfoProvider>().To<ApplicationDeploymentWrapper>().InSingletonScope();
#endif
        }

        public void PerfornStartupTasks()
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
