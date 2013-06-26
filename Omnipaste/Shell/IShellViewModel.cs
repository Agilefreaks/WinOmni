namespace Omnipaste.Shell
{
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.ContextMenu;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.UserToken;

    public interface IShellViewModel : IScreen, IConductActiveItem, IHandle<GetTokenFromUserMessage>, IHandle<TokenRequestResultMessage>, IHandle<ConfigurationCompletedMessage>
    {
        IUserTokenViewModel UserTokenViewModel { get; set; }

        [Inject]
        IContextMenuViewModel ContextMenuViewModel { get; set; }
    }
}