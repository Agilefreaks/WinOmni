namespace Omnipaste.Shell
{
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.ContextMenu;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.UserToken;
    using WindowsClipboard.Interfaces;

    public interface IShellViewModel : IScreen, IConductActiveItem, IDelegateClipboardMessageHandling, IHandle<GetTokenFromUserMessage>, IHandle<TokenRequestResultMessage>, IHandle<ConfigurationCompletedMessage>
    {
        IUserTokenViewModel UserTokenViewModel { get; set; }

        [Inject]
        IContextMenuViewModel ContextMenuViewModel { get; set; }
    }
}