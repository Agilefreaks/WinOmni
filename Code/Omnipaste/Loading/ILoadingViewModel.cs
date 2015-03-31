namespace Omnipaste.Loading
{
    using Caliburn.Micro;
    using Omnipaste.Framework.EventAggregatorMessages;
    using Omnipaste.Loading.UserToken;

    public interface ILoadingViewModel : IConductActiveItem, IHandle<GetTokenFromUserMessage>, IHandle<TokenRequestResultMessage>,
        IHandle<ActivationFailedMessage>, IHandle<ShowAndroidInstallGuideMessage>, IHandle<AndroidInstallationCompleteMessage>,
        IHandle<ShowConnectionTroubleshooterMessage>, IHandle<ShowCongratulationsMessage>, IHandle<ShowCreateClippingMessage>
    {
        LoadingViewModelStateEnum State { get; set; }

        IUserTokenViewModel UserTokenViewModel { get; set; }

        ILoadingViewModel Loading();
    }
}