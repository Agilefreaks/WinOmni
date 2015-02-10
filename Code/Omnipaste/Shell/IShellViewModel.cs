namespace Omnipaste.Shell
{
    using System;
    using Caliburn.Micro;
    using Omnipaste.Dialog;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Loading;
    using Omnipaste.Shell.ContextMenu;
    using Omnipaste.Shell.SideMenu;
    using OmniUI.Workspace;

    public interface IShellViewModel : IConductActiveItem,
                                       IScreen,
                                       IViewAware,
                                       IWorkspaceConductor,
                                       IHandle<ShowShellMessage>,
                                       IHandle<RetryMessage>,
                                       IDisposable
    {
        #region Public Properties

        IContextMenuViewModel ContextMenuViewModel { get; set; }

        IDialogViewModel DialogViewModel { get; set; }

        ILoadingViewModel LoadingViewModel { get; set; }

        ISideMenuViewModel SideMenuViewModel { get; set; }

        #endregion

        #region Public Methods and Operators

        void Close();

        void Show();

        #endregion
    }
}