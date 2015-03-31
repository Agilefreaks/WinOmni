namespace Omnipaste.Conversations.Conversation
{
    using System.Collections.ObjectModel;
    using Omnipaste.Framework.Models;
    using OmniUI.Details;

    public interface IConversationHeaderViewModel : IDetailsViewModel
    {
        ConversationHeaderStateEnum State { get; set; }

        ObservableCollection<ContactModel> Recipients { get; set; }
    }
}