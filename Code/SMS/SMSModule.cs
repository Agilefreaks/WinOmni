namespace SMS
{
    using Ninject.Modules;
    using SMS.Resources.v1;

    public class SMSModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<ISMSMessages>().To<SMSMessages>().InSingletonScope();
        }
    }
}