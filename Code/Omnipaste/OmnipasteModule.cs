namespace Omnipaste
{
    using System;
    using System.Collections.Generic;
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon;
    using OmniCommon.DataProviders;
    using OmniCommon.Interfaces;
    using Omnipaste.DataProviders;
    using Omnipaste.Dialog;
    using Omnipaste.NotificationList;
    using Omnipaste.Services;
    using Omnipaste.Services.ActivationServiceData;
    using Omnipaste.Services.Connectivity;
    using Omnipaste.Shell;
    using Omnipaste.Shell.Connection;
    using Omnipaste.Shell.Settings;
    using Omnipaste.Shell.SettingsHeader;
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
            Kernel.Bind<IArgumentsProvider>().To<EnvironmentArgumentsProvider>();
            Kernel.Bind<IArgumentsDataProvider>().To<ArgumentsDataProvider>();

            Kernel.Bind<IFlyoutViewModel>().ToMethod(context => context.Kernel.Get<ISettingsViewModel>());
            Kernel.Bind<IHeaderButtonViewModel>().ToMethod(context => context.Kernel.Get<ISettingsHeaderViewModel>());
            Kernel.Bind<IHeaderButtonViewModel>().ToMethod(context => context.Kernel.Get<IConnectionViewModel>());
        }

        protected override IEnumerable<Type> GenerateSingletonTypesList()
        {
            return new[]
                       {
                           typeof(ShellViewModel), typeof(DialogViewModel), typeof(SettingsViewModel),
                           typeof(NotificationViewModelFactory), typeof(ConnectivityHelper),
                           typeof(ISettingsHeaderViewModel), typeof(IConnectionViewModel)
                       };
        }
    }
}