namespace Omnipaste
{
    using System;
    using System.Collections.Generic;
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.DataProviders;
    using OmniCommon.Interfaces;
    using Omnipaste.DataProviders;
    using Omnipaste.Dialog;
    using Omnipaste.NotificationList;
    using Omnipaste.Services;
    using Omnipaste.Services.ActivationServiceData;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;
    using Omnipaste.Services.Monitors.Internet;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps.ProxyDetection;
    using Omnipaste.Services.Monitors.Power;
    using Omnipaste.Services.Monitors.User;
    using Omnipaste.Shell;
    using Omnipaste.Shell.Connection;
    using Omnipaste.Shell.Settings;
    using Omnipaste.Shell.SettingsHeader;
    using OmniUI;
    using OmniUI.Flyout;
    using OmniUI.HeaderButton;

    public class OmnipasteModule : ModuleBase
    {
        protected override void LoadCore()
        {
            Kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
            Kernel.Bind<ISessionManager>().To<SessionManager>().InSingletonScope();
            Kernel.Bind<IStepFactory>().To<StepFactory>().InSingletonScope();

            Kernel.Bind<IConfigurationProvider>().To<DPAPIConfigurationProvider>().InSingletonScope();
            Kernel.Bind<IWindowHandleProvider>().To<WindowHandleProvider>().InSingletonScope();
            Kernel.Bind<IArgumentsProvider>().To<EnvironmentArgumentsProvider>();
            Kernel.Bind<IArgumentsDataProvider>().To<ArgumentsDataProvider>();

            Kernel.Bind<IFlyoutViewModel>().ToMethod(context => context.Kernel.Get<ISettingsViewModel>());
            Kernel.Bind<IHeaderButtonViewModel>().ToMethod(context => context.Kernel.Get<ISettingsHeaderViewModel>());
            Kernel.Bind<IHeaderButtonViewModel>().ToMethod(context => context.Kernel.Get<IConnectionViewModel>());

            Kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            Kernel.Bind<IProxyConfigurationDetector>().To<HttpProxyConfigurationDetector>();
            Kernel.Bind<IProxyConfigurationDetector>().To<SocksProxyConfigurationDetector>();
        }

        protected override IEnumerable<Type> GenerateSingletonTypesList()
        {
            return new[]
                       {
                           typeof(ShellViewModel), typeof(DialogViewModel), typeof(SettingsViewModel),
                           typeof(NotificationViewModelFactory), typeof(ConnectivityHelper),
                           typeof(SettingsHeaderViewModel), typeof(ConnectionViewModel), typeof(ActivationSequenceProvider),
                           typeof(WebProxyFactory), typeof(SystemPowerHelper),
                           typeof(InternetConnectivityMonitor), typeof(PowerMonitor), typeof(UserMonitor),
                           typeof(ConnectionEventSupervisor)
                       };
        }
    }
}