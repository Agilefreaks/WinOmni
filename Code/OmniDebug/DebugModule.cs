namespace OmniDebug
{
    using Castle.Core.Internal;
    using Ninject;
    using Ninject.Modules;
    using OmniSync;

    public class DebugModule : NinjectModule
    {
        public override void Load()
        {
            var existingWebsocketConnectionFactory = Kernel.Get<IWebsocketConnectionFactory>();
            var bindings = Kernel.GetBindings(typeof(IWebsocketConnectionFactory));
            bindings.ForEach(binding => Kernel.RemoveBinding(binding));

            Kernel.Bind<IWebsocketConnectionFactory>()
                .ToConstant(new WebsocketConnectionFactoryWrapper(existingWebsocketConnectionFactory))
                .InSingletonScope();
        }
    }
}