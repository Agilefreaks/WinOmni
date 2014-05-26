namespace Omnipaste.Shell
{
    using Caliburn.Micro;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.EventAggregatorMessages;

    public interface IShellViewModel : IScreen, IConductActiveItem, IHandle<GetTokenFromUserMessage>, IHandle<TokenRequestResultMessage>, IHandle<ConfigurationCompletedMessage>
    {
    }
}