namespace CustomizedClickOnce.Common
{
    using Ninject.Modules;

    public class CustomizedClickOnceCommonModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IClickOnceHelper>().To<ClickOnceHelper>().InSingletonScope();
        }
    }
}