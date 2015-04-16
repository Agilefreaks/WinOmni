namespace Omnipaste.Shell.SideMenu
{
    using System;
    using System.Collections.Generic;
    using Omnipaste.Shell.SessionInfo;
    using OmniUI.Menu.MainItem;
    using OmniUI.Menu.SecondaryItem;

    public interface ISideMenuViewModel : IDisposable
    {
        ISessionInfoViewModel SessionInfoViewModel { get; set; }

        IEnumerable<IMainItemViewModel> MainMenuViewModels { get; set; }

        IEnumerable<ISecondaryItemViewModel> SecondaryMenuViewModels { get; set; }
    }
}
