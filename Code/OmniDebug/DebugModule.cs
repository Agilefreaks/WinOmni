namespace OmniDebug
{
    using System;
    using System.Collections.Generic;
    using Caliburn.Micro;
    using Castle.Core.Internal;
    using Ninject;
    using OmniCommon;
    using OmniDebug.DebugBar;
    using OmniDebug.DebugHeader;
    using OmniSync;
    using OmniUI.Flyout;
    using OmniUI.HeaderButton;

    public class DebugModule : ModuleBase
    {
        public override void ReplaceExistingBindings()
        {
            var existingWebsocketConnectionFactory = Kernel.Get<IWebsocketConnectionFactory>();
            var bindings = Kernel.GetBindings(typeof(IWebsocketConnectionFactory));
            bindings.ForEach(binding => Kernel.RemoveBinding(binding));

            Kernel.Bind<IWebsocketConnectionFactory>()
                .ToConstant(new WebsocketConnectionFactoryWrapper(existingWebsocketConnectionFactory))
                .InSingletonScope();
        }

        public override void LoadCore()
        {
            AssemblySource.Instance.Add(GetType().Assembly);
            Kernel.Bind<IFlyoutViewModel>().ToMethod(context => context.Kernel.Get<IDebugBarViewModel>());
            Kernel.Bind<IHeaderButtonViewModel>().ToMethod(context => context.Kernel.Get<IDebugHeaderViewModel>());
        }

        protected override IEnumerable<Type> GenerateSingleTypesList()
        {
            return new[] { typeof(DebugHeaderViewModel), typeof(DebugBarViewModel) };
        }
    }
}