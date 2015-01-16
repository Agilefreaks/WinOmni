namespace SMS
{
    using Ninject;
    using Ninject.Modules;
    using OmniCommon.Interfaces;
    using SMS.Handlers;
    using SMS.Resources.v1;

    public class SMSModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<ISMSMessages>().To<SMSMessages>().InSingletonScope();

            Kernel.Bind<ISmsMessageCreatedHandler>().To<SmsMessageCreatedHandler>().InSingletonScope();
            Kernel.Bind<IHandler>().ToMethod(context => context.Kernel.Get<ISmsMessageCreatedHandler>());
        }
    }
}