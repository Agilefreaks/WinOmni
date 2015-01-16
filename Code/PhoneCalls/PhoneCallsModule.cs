namespace PhoneCalls
{
    using Ninject.Modules;
    using PhoneCalls.Resources.v1;

    public class PhoneCallsModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IPhoneCalls>().To<PhoneCalls>().InSingletonScope();
        }
    }
}