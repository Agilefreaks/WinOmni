namespace OmniDebug
{
    using System;
    using System.Collections.Generic;
    using Caliburn.Micro;
    using Ninject;
    using Omni;
    using OmniCommon;
    using OmniDebug.DebugBar;
    using OmniDebug.DebugHeader;
    using OmniDebug.Services;
    using OmniSync;
    using OmniUI.Flyout;
    using OmniUI.HeaderButton;

    public class DebugModule : ModuleBase
    {
        #region Fields

        #endregion

        #region Methods

        protected override IEnumerable<Type> GenerateSingletonTypesList()
        {
            return new[] { typeof(DebugHeaderViewModel), typeof(DebugBarViewModel) };
        }

        protected override void LoadCore()
        {
            AssemblySource.Instance.Add(GetType().Assembly);

            Kernel.Bind<IWebsocketConnectionFactory>().To<WebsocketConnectionFactoryWrapper>().InSingletonScope();

            Kernel.Bind<IOmniServiceWrapper>().To<OmniServiceWrapper>().InSingletonScope();
            Kernel.Bind<IOmniService>().ToMethod(context => context.Kernel.Get<IOmniServiceWrapper>());

            Kernel.Bind<IFlyoutViewModel>().ToMethod(context => context.Kernel.Get<IDebugBarViewModel>());
            Kernel.Bind<IHeaderButtonViewModel>().ToMethod(context => context.Kernel.Get<IDebugHeaderViewModel>());
        }

        protected override IEnumerable<Type> TypesToOverriderBindingsFor()
        {
            return new[] { typeof(IWebsocketConnectionFactory), typeof(IOmniService) };
        }

        #endregion
    }
}