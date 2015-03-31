namespace Omnipaste.Conversations
{
    using Omnipaste.Conversations.ContactList;
    using OmniUI.Workspaces;

    public interface IConversationWorkspace : IMasterDetailsWorkspace
    {
        new IContactListViewModel MasterScreen { get; }
    }
}