namespace Omnipaste.Shell
{
    using Caliburn.Micro;
    using Omnipaste.Dialog;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Loading;

    public interface IShellViewModel : IConductor, IScreen, IViewAware, IHandle<ConfigurationCompletedMessage>
    {
        ILoadingViewModel LoadingViewModel { get; set; }

        IDialogViewModel DialogViewModel { get; set; }
    }
}