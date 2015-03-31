namespace Omnipaste.Workspaces.People
{
    using Omnipaste.ContactList;
    using OmniUI.Workspace;

    public interface IPeopleWorkspace : IMasterDetailsWorkspace
    {
        new IContactListViewModel MasterScreen { get; }
    }
}