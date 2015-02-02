namespace Omnipaste.WorkspaceDetails.Conversation.Message
{
    using OmniCommon.Interfaces;
    using Omnipaste.DetailsViewModel;
    using Omnipaste.Models;
    using Omnipaste.Services;

    public class MessageViewModel : ConversationItemViewModel<Message>, IMessageViewModel
    {
        #region Constructors and Destructors

        public MessageViewModel(IUiRefreshService uiRefreshService, IConfigurationService configurationService)
            : base(uiRefreshService, configurationService)
        {
        }

        #endregion
    }
}