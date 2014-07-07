namespace Omnipaste.Shell
{
    using Caliburn.Micro;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.Dialog;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Loading;
    using Omnipaste.Shell.ContextMenu;

    public interface IShellViewModel : IConductor, IScreen, IViewAware, IHandle<ShowShellMessage>, IHandle<RetryMessage>
    {
        ILoadingViewModel LoadingViewModel { get; set; }

        IDialogViewModel DialogViewModel { get; set; }

        IContextMenuViewModel ContextMenuViewModel { get; set; }

        void Show();

        void Close(bool showBaloon);
    }
}