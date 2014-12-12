namespace Omnipaste.Shell
{
    using System;
    using Caliburn.Micro;
    using Omnipaste.Dialog;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Loading;
    using Omnipaste.Shell.ContextMenu;
    using Omnipaste.Shell.SideMenu;

    public interface IShellViewModel : IConductActiveItem, IScreen, IViewAware, IHandle<ShowShellMessage>, IHandle<RetryMessage>, IDisposable
    {
        ILoadingViewModel LoadingViewModel { get; set; }

        IDialogViewModel DialogViewModel { get; set; }

        IContextMenuViewModel ContextMenuViewModel { get; set; }

        ISideMenuViewModel SideMenuViewModel { get; set; }

        void Show();

        void Close();
    }
}