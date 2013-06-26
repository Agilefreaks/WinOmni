namespace Omnipaste.Shell
{
    using Caliburn.Micro;
    using OmniCommon.EventAggregatorMessages;

    public interface IShellViewModel : IConductor, IHandle<GetTokenFromUserMessage>, IHandle<TokenRequestResultMessage>
    {
    }
}