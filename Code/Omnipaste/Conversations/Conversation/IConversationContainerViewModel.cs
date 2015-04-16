namespace Omnipaste.Conversations.Conversation
{
    using System.Collections.ObjectModel;
    using Omnipaste.Conversations.Conversation.SMSComposer;
    using Omnipaste.Framework.Models;
    using OmniUI.Details;

    public interface IConversationContainerViewModel : IDetailsViewModel
    {
        ObservableCollection<ContactModel> Recipients { get; set; }

        IConversationContentViewModel ConversationContentViewModel { get; set; }

        ISMSComposerViewModel SMSComposer { get; set; }
    }
}