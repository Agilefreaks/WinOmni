namespace Omnipaste.Shell.SideMenu
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core.Internal;
    using Ninject;
    using OmniCommon.Interfaces;
    using Omnipaste.Shell.SessionInfo;
    using OmniUI.MainMenuEntry;
    using OmniUI.SecondaryMenuEntry;

    public class SideMenuViewModel : ISideMenuViewModel
    {
        [Inject]
        public ISessionInfoViewModel SessionInfoViewModel { get; set; }

        [Inject]
        public IEnumerable<IMainMenuEntryViewModel> MainMenuViewModels { get; set; }
        
        [Inject]
        public IEnumerable<ISecondaryMenuEntryViewModel> SecondaryMenuViewModels { get; set; }

        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        public string AppNameAndVersion
        {
            get
            {
                return ConfigurationService.AppNameAndVersion;
            }
        }

        public void Dispose()
        {
            MainMenuViewModels.Cast<IDisposable>()
                .Concat(SecondaryMenuViewModels)
                .Concat(new[] { SessionInfoViewModel })
                .ForEach(disposable => disposable.Dispose());
        }
    }
}