namespace OmniSync
{
    using Newtonsoft.Json.Linq;
    using Ninject.Modules;
    using WampSharp.V1;

    public class OmniSyncModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IWampChannelFactory<JToken>>().To<DefaultWampChannelFactory>().InSingletonScope();
            Kernel.Bind<IWebsocketConnectionFactory>().To<WebsocketConnectionFactory>().InSingletonScope();
        }
    }
}