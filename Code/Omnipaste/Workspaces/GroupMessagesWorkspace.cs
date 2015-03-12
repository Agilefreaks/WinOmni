namespace Omnipaste.Workspaces
{
    using Caliburn.Micro;
    using Omnipaste.ContactList;
    using OmniUI.Attributes;
    using OmniUI.Workspace;

    [UseView(typeof(WorkspaceView))]
    public class GroupMessagesWorkspace : MasterDetailsWorkspace, IGroupMessagesWorkspace
    {
        public GroupMessagesWorkspace(IContactListViewModel masterScreen, IDetailsConductorViewModel detailsConductor)
            : base(masterScreen, detailsConductor)
        {
        }

        public override string DisplayName
        {
            get
            {
                return "+ New message";
            }
        }

        public new IContactListViewModel MasterScreen { get; private set; }

        IScreen IMasterDetailsWorkspace.MasterScreen
        {
            get
            {
                return MasterScreen;
            }
        }

    }
}