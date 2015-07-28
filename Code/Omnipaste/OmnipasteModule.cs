namespace Omnipaste
{
    using System;
    using System.Collections.Generic;
    using Caliburn.Micro;
    using Ninject;
    using Ninject.Activation;
    using OmniApi.Support;
    using OmniCommon.DataProviders;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using OmniCommon.Settings;
    using Omnipaste.Activities;
    using Omnipaste.Activities.ActivityList;
    using Omnipaste.Activities.Menu;
    using Omnipaste.Clippings;
    using Omnipaste.Clippings.ClippingList;
    using Omnipaste.Clippings.Menu;
    using Omnipaste.Conversations;
    using Omnipaste.Conversations.ContactList;
    using Omnipaste.Conversations.Menu;
    using Omnipaste.Framework;
    using Omnipaste.Framework.DataProviders;
    using Omnipaste.Framework.Entities.Factories;
    using Omnipaste.Framework.Models.Factories;
    using Omnipaste.Framework.Services;
    using Omnipaste.Framework.Services.ActivationServiceData;
    using Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps;
    using Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps.ProxyDetection;
    using Omnipaste.Framework.Services.ExceptionReporters;
    using Omnipaste.Framework.Services.Monitors.Credentials;
    using Omnipaste.Framework.Services.Monitors.Internet;
    using Omnipaste.Framework.Services.Monitors.Power;
    using Omnipaste.Framework.Services.Monitors.ProxyConfiguration;
    using Omnipaste.Framework.Services.Monitors.User;
    using Omnipaste.Framework.Services.Providers;
    using Omnipaste.Framework.Services.Repositories;
    using Omnipaste.Notifications.NotificationList;
    using Omnipaste.Profile;
    using Omnipaste.Shell;
    using Omnipaste.Shell.SessionInfo;
    using Omnipaste.Shell.Settings;
    using Omnipaste.Shell.TitleBar;
    using OmniUI;
    using OmniUI.Dialog;
    using OmniUI.Flyout;
    using OmniUI.Menu.MainItem;
    using OmniUI.Menu.SecondaryItem;
    using OmniUI.Menu.TitleBarItem;
    using OmniUI.Workspaces;

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
            Kernel.Bind<IMainItemViewModel>().To<ActivityItemViewModel>().InSingletonScope();
            Kernel.Bind<IMainItemViewModel>().To<PeopleItemViewModel>().InSingletonScope();
            Kernel.Bind<IMainItemViewModel>().To<ClippingsItemViewModel>().InSingletonScope();
            Kernel.Bind<IMainItemViewModel>().To<NewMessageItemViewModel>().InSingletonScope();
            Kernel.Bind<ISecondaryItemViewModel>()
                .ToMethod(context => context.Kernel.Get<SettingsItemViewModel>());
            Kernel.Bind<ITitleBarItemViewModel>().To<NewVersionTitleBarItemViewModel>().InSingletonScope();

            Kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            Kernel.Bind<IProxyConfigurationDetector>().To<HttpProxyConfigurationDetector>();
            Kernel.Bind<IProxyConfigurationDetector>().To<SocksProxyConfigurationDetector>();
            Kernel.Bind<IWorkspaceConductor>().ToMethod(context => context.Kernel.Get<IShellViewModel>());

            Kernel.Bind<IExceptionReporter>().ToMethod(GetExceptionReporter).InSingletonScope();
            Kernel.Bind<IUpdateManager>().ToConstant(new NAppUpdateManager());
            Kernel.Bind<ILogger>().ToConstant(NLogAdapter.Instance).InSingletonScope();
        }

        protected override IEnumerable<Type> GenerateSingletonTypesList()
        {
            return new[]
                       {
                           typeof(ShellViewModel), typeof(DialogViewModel), typeof(SettingsViewModel),
                           typeof(NotificationViewModelFactory), typeof(ConnectivityHelper), typeof(SessionInfoViewModel),
                           typeof(ActivationSequenceProvider), typeof(WebProxyFactory), typeof(SystemPowerHelper),
                           typeof(InternetConnectivityMonitor), typeof(PowerMonitor), typeof(UserMonitor), typeof(CredentialsMonitor),
                           typeof(ProxyConfigurationMonitor), typeof(ConnectivitySupervisor), typeof(EntitySupervisor),
                           typeof(EntitySupervisor), typeof(ClippingRepository), typeof(PhoneCallRepository), typeof(ContactRepository), typeof(UpdateRepository), typeof(SmsMessageRepository),
                           typeof(ActivityWorkspace), typeof(NewMessageWorkspace), typeof(ConversationWorkspace), typeof(ClippingsWorkspace), typeof(DetailsViewModelFactory), typeof(ProfileWorkspace),
                           typeof(ActivityViewModelFactory), typeof(ContactViewModelFactory), typeof(ClippingViewModelFactory), typeof(ConversationModelFactory),
                           typeof(ConversationProvider), 
                           typeof(SmsMessageFactory), typeof(PhoneCallFactory), typeof(ContactFactory), typeof(ActivityModelFactory), typeof(PhoneCallModelFactory), typeof(SmsMessageModelFactory)
                       };
        }

        private static CompositeExceptionReporter GetExceptionReporter(IContext context)
        {
            var bugFreakExceptionReporter = context.Kernel.Get<BugFreakExceptionReporter>();
            var logExceptionReporter = context.Kernel.Get<LogExceptionReporter>();

            return new CompositeExceptionReporter(bugFreakExceptionReporter, logExceptionReporter);
        }
    }
}