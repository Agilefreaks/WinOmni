namespace Omnipaste.Loading
{
    using Caliburn.Micro;
    using OmniCommon.EventAggregatorMessages;

    public interface ILoadingViewModel : IScreen, IHandle<GetTokenFromUserMessage>, IHandle<>
    {
        LoadingViewModelStateEnum State { get; set; }
    }
}