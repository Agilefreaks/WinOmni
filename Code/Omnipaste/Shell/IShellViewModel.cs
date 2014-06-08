namespace Omnipaste.Shell
{
    using System.ComponentModel;
    using Caliburn.Micro;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.Dialog;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Loading;

    public interface IShellViewModel : IConductor, IScreen, IViewAware, IHandle<GetTokenFromUserMessage>, IHandle<ConfigurationCompletedMessage>
    {
        ILoadingViewModel LoadingViewModel { get; set; }

        IDialogViewModel DialogViewModel { get; set; }
    }
}