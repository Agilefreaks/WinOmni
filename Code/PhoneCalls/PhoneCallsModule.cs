namespace PhoneCalls
{
    using Ninject;
    using Ninject.Modules;
    using OmniCommon.Interfaces;
    using PhoneCalls.Handlers;
    using PhoneCalls.Resources.v1;

    public class PhoneCallsModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IPhoneCalls>().To<PhoneCalls>().InSingletonScope();

            Kernel.Bind<IPhoneCallReceivedHandler>().To<PhoneCallReceivedHandler>().InSingletonScope();
            Kernel.Bind<IHandler>().ToMethod(context => context.Kernel.Get<IPhoneCallReceivedHandler>());
        }
    }
}