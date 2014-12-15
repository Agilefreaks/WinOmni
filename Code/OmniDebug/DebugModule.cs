namespace OmniDebug
{
    using System;
    using System.Collections.Generic;
    using Clipboard.API.Resources.v1;
    using Events.Api.Resources.v1;
    using Ninject;
    using Omni;
    using OmniDebug.DebugBar;
    using OmniDebug.DebugBar.IncomingClipping;
    using OmniDebug.DebugBar.PhoneNotification;
    using OmniDebug.DebugBar.SMSNotification;
    using OmniDebug.Services;
    using OmniSync;
    using OmniUI;
    using OmniUI.Flyout;
    using OmniUI.SecondaryMenuEntry;

    public class DebugModule : ModuleBase
    {
        #region Fields

        #endregion

        #region Methods

        protected override IEnumerable<Type> GenerateSingletonTypesList()
        {
            return new[]
            {
                typeof (DebugBarViewModel)
            };
        }

        protected override void LoadCore()
        {
            Kernel.Bind<IWebsocketConnectionFactory>().To<WebsocketConnectionFactoryWrapper>().InSingletonScope();

            Kernel.Bind<IOmniServiceWrapper>().To<OmniServiceWrapper>().InSingletonScope();
            Kernel.Bind<IOmniService>().ToMethod(context => context.Kernel.Get<IOmniServiceWrapper>());

            Kernel.Bind<IFlyoutViewModel>().ToMethod(context => context.Kernel.Get<IDebugBarViewModel>());
            Kernel.Bind<ISecondaryMenuEntryViewModel>().ToMethod(context => context.Kernel.Get<DebugMenuEntryViewModel>());

            Kernel.Bind<EventsWrapper>()
                .ToConstructor(syntax => new EventsWrapper(syntax.Context.Kernel.Get<Events>()))
                .InSingletonScope();
            Kernel.Bind<IEventsWrapper>().ToMethod(context => context.Kernel.Get<EventsWrapper>());
            Kernel.Bind<IEvents>().ToMethod(context => context.Kernel.Get<IEventsWrapper>());

            Kernel.Bind<ClippingsWrapper>()
                .ToConstructor(syntax => new ClippingsWrapper(syntax.Context.Kernel.Get<Clippings>()))
                .InSingletonScope();
            Kernel.Bind<IClippingsWrapper>().ToMethod(context => context.Kernel.Get<ClippingsWrapper>()).InSingletonScope();
            Kernel.Bind<IClippings>().ToMethod(context => context.Kernel.Get<IClippingsWrapper>());

            Kernel.Bind<IDebugBarPanel>().To<SMSNotificationViewModel>();
            Kernel.Bind<IDebugBarPanel>().To<PhoneNotificationViewModel>();
            Kernel.Bind<IDebugBarPanel>().To<IncomingClippingViewModel>();
        }

        protected override IEnumerable<Type> TypesToOverriderBindingsFor()
        {
            return new[] { typeof(IWebsocketConnectionFactory), typeof(IOmniService), typeof(IEvents), typeof(IClippings) };
        }

        #endregion
    }
}