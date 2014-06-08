namespace Omnipaste.Shell
{
    using Caliburn.Micro;
    using Omnipaste.Dialog;
    using Omnipaste.Loading;
    using Omnipaste.Shell.ContextMenu;

    public interface IShellViewModel : IConductor, IScreen, IViewAware
    {
        ILoadingViewModel LoadingViewModel { get; set; }

        IDialogViewModel DialogViewModel { get; set; }

        IContextMenuViewModel ContextMenu { get; set; }

        void Show();
    }
}