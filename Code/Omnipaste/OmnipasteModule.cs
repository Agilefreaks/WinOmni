namespace Omnipaste
{
    using System;
    using System.Collections.Generic;
    using Caliburn.Micro;
    using Ninject;
    using OmniApi.Support;
    using OmniCommon.DataProviders;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using OmniCommon.Settings;
    using Omnipaste.Activity;
    using Omnipaste.ActivityDetails;
    using Omnipaste.DataProviders;
    using Omnipaste.Dialog;
    using Omnipaste.Models;
    using Omnipaste.NotificationList;
    using Omnipaste.Services;
    using Omnipaste.Services.ActivationServiceData;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps.ProxyDetection;
    using Omnipaste.Services.Monitors.Credentials;
    using Omnipaste.Services.Monitors.Internet;
    using Omnipaste.Services.Monitors.Power;
    using Omnipaste.Services.Monitors.ProxyConfiguration;
    using Omnipaste.Services.Monitors.User;
    using Omnipaste.Services.Repositories;
    using Omnipaste.Shell;
    using Omnipaste.Shell.SessionInfo;
    using Omnipaste.Shell.Settings;
    using Omnipaste.Shell.TitleBar;
    using Omnipaste.Workspaces;
    using OmniUI;
    using OmniUI.Flyout;
    using OmniUI.MainMenuEntry;
    using OmniUI.SecondaryMenuEntry;
    using OmniUI.TitleBarItem;
    using OmniUI.Workspace;

    public class OmnipasteModule : ModuleBase
    {
        protected override void LoadCore()
        {
            Kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
            Kernel.Bind<ISessionManager>().To<SessionManager>().InSingletonScope();
            Kernel.Bind<IHttpResponseMessageHandler>().To<SessionManager>().InSingletonScope();
            Kernel.Bind<IStepFactory>().To<StepFactory>().InSingletonScope();

            Kernel.Bind<IConfigurationContainer>().To<DPAPIConfigurationContainer>().InSingletonScope();
            Kernel.Bind<IWindowHandleProvider>().To<WindowHandleProvider>().InSingletonScope();
            Kernel.Bind<IArgumentsProvider>().To<EnvironmentArgumentsProvider>();
            Kernel.Bind<IArgumentsDataProvider>().To<ArgumentsDataProvider>();

            Kernel.Bind<IFlyoutViewModel>().ToMethod(context => context.Kernel.Get<ISettingsViewModel>());
            Kernel.Bind<IMainMenuEntryViewModel>().To<ActivityMenuEntryViewModel>().InSingletonScope();
            Kernel.Bind<IMainMenuEntryViewModel>().To<EventsMenuEntryViewModel>().InSingletonScope();
            Kernel.Bind<IMainMenuEntryViewModel>().To<ClippingsMenuEntryViewModel>().InSingletonScope();
            Kernel.Bind<ISecondaryMenuEntryViewModel>().ToMethod(context => context.Kernel.Get<SettingsMenuEntryViewModel>());
            Kernel.Bind<ITitleBarItemViewModel>().To<NewVersionTitleBarItemViewModel>().InSingletonScope();

            Kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            Kernel.Bind<IProxyConfigurationDetector>().To<HttpProxyConfigurationDetector>();
            Kernel.Bind<IProxyConfigurationDetector>().To<SocksProxyConfigurationDetector>();
            Kernel.Bind<IWorkspaceConductor>().ToMethod(context => context.Kernel.Get<IShellViewModel>());

            Kernel.Bind<IExceptionReporter>().To<BugFreakExceptionReporter>().InSingletonScope();
            Kernel.Bind<InMemoryStore>().ToSelf().InSingletonScope();
            Kernel.Bind<IMessageStore>().ToMethod(context => context.Kernel.Get<InMemoryStore>());
            Kernel.Bind<ICallStore>().ToMethod(context => context.Kernel.Get<InMemoryStore>());
            Kernel.Bind<IUpdateManager>().ToConstant(new NAppUpdateManager());
        }

        protected override IEnumerable<Type> GenerateSingletonTypesList()
        {
            return new[]
                       {
                           typeof(ShellViewModel), typeof(DialogViewModel), typeof(SettingsViewModel),
                           typeof(NotificationViewModelFactory), typeof(ConnectivityHelper),
                           typeof(SessionInfoViewModel), typeof(ActivationSequenceProvider),
                           typeof(WebProxyFactory), typeof(SystemPowerHelper),
                           typeof(InternetConnectivityMonitor), typeof(PowerMonitor), typeof(UserMonitor), 
                           typeof(ProxyConfigurationMonitor), typeof(ConnectivitySupervisor), 
                           typeof(EntitySupervisor), typeof(ClippingRepository), typeof(SMSMessageFactory),
                           typeof(ActivityWorkspace), typeof(EventsWorkspace), typeof(ClippingWorkspace),
                           typeof(CredentialsMonitor), typeof(ActivityViewModelFactory), typeof(ActivityDetailsViewModelFactory)
                       };
        }
    }
}