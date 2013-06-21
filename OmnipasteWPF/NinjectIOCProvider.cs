namespace OmnipasteWPF
{
    using System;
    using System.Linq;
    using Cinch;
    using Ninject;
    using Ninject.Parameters;
    using OmniCommon.Services.ActivationServiceData;

    public abstract class NinjectIOCProvider : IIOCProvider, IDependencyResolver
    {
        protected readonly StandardKernel Kernel;

        protected NinjectIOCProvider()
        {
            Kernel = new StandardKernel();
        }

        public virtual void SetupContainer()
        {
            RegisterDefaultServices();
        }

        public T GetTypeFromContainer<T>()
        {
            return Kernel.Get<T>();
        }

        public object Get(Type type, params DependencyParameter[] parameters)
        {
            var injectionParameters =
                parameters.Select(p => new ConstructorArgument(p.Name, p.Value, false) as IParameter).ToArray();

            return Kernel.Get(type, injectionParameters);
        }

        private void RegisterDefaultServices()
        {
            Kernel.Bind<IKernel>().ToConstant(Kernel);
            Kernel.Bind<IIOCProvider>().ToConstant(this);
            Kernel.Bind<ILogger>().To<NullLogger>().InSingletonScope();
            Kernel.Bind<IUIVisualizerService>().To<WPFUIVisualizerService>().InSingletonScope();
            Kernel.Bind<IMessageBoxService>().To<WPFMessageBoxService>().InSingletonScope();
            Kernel.Bind<IOpenFileService>().To<WPFOpenFileService>().InSingletonScope();
            Kernel.Bind<ISaveFileService>().To<WPFSaveFileService>().InSingletonScope();
        }
    }
}