namespace OmniSync
{
    using Newtonsoft.Json.Linq;
    using Ninject.Modules;
    using WampSharp;

    public class OmniSyncModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IWampChannelFactory<JToken>>().To<DefaultWampChannelFactory>().InSingletonScope();
            Kernel.Bind<INotificationService>().To<NotificationService>().InSingletonScope();
        }
    }
}