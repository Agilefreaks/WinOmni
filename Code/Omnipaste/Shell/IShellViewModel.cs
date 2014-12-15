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

    public interface IShellViewModel : IConductActiveItem, IScreen, IViewAware, IWorkspaceConductor, IHandle<ShowShellMessage>, IHandle<RetryMessage>, IDisposable
    {
        ILoadingViewModel LoadingViewModel { get; set; }

        IDialogViewModel DialogViewModel { get; set; }

        IContextMenuViewModel ContextMenuViewModel { get; set; }

        ISideMenuViewModel SideMenuViewModel { get; set; }

        void Show();

        void Close();
    }
}