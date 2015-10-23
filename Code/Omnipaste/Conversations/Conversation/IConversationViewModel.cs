namespace Omnipaste.Conversations.Conversation
{
    using System.Collections.ObjectModel;
    using Omnipaste.Framework.Models;
    using OmniUI.Details;

    public interface IConversationViewModel : IDetailsViewModelWithHeader
    {
        ObservableCollection<ContactModel> Recipients { get; set; }

        ConversationViewModelStateEnum State { get; set; }
    }
}