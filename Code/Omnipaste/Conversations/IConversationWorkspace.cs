namespace Omnipaste.Conversations
{
    using Omnipaste.Conversations.ContactList;
    using Omnipaste.Framework;
    using OmniUI.Workspaces;

    public interface IConversationWorkspace : IMasterDetailsWorkspace
    {
        IContactListViewModel ContactListViewModel { get; }

        IDetailsViewModelFactory DetailsViewModelFactory { get; set; }
    }
}