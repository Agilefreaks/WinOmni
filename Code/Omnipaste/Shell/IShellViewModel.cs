namespace Omnipaste.Shell
{
    using Caliburn.Micro;
    using Omnipaste.Dialog;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Loading;
    using Omnipaste.Shell.ContextMenu;

    public interface IShellViewModel : IConductor, IScreen, IViewAware, IHandle<ShowShellMessage>
    {
        ILoadingViewModel LoadingViewModel { get; set; }

        IDialogViewModel DialogViewModel { get; set; }

        IContextMenuViewModel ContextMenuViewModel { get; set; }

        void Show();
    }
}