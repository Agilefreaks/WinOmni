namespace Omnipaste.Shell.SideMenu
{
    using System;
    using System.Collections.Generic;
    using Omnipaste.Shell.SessionInfo;
    using OmniUI.MainMenuEntry;
    using OmniUI.SecondaryMenuEntry;

    public interface ISideMenuViewModel : IDisposable
    {
        ISessionInfoViewModel SessionInfoViewModel { get; set; }

        IEnumerable<IMainMenuEntryViewModel> MainMenuViewModels { get; set; }

        IEnumerable<ISecondaryMenuEntryViewModel> SecondaryMenuViewModels { get; set; }
    }
}
