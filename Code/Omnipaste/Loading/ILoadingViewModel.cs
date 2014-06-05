namespace Omnipaste.Loading
{
    using Caliburn.Micro;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.EventAggregatorMessages;

    public interface ILoadingViewModel : IScreen, IHandle<GetTokenFromUserMessage>, IHandle<ConfigurationCompletedMessage>
    {
        LoadingViewModelStateEnum State { get; set; }
    }
}