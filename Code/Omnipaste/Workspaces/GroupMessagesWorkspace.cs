namespace Omnipaste.Workspaces
{
    using Omnipaste.ContactList;
    using Omnipaste.Properties;
    using OmniUI.Attributes;
    using OmniUI.Workspace;

    [UseView(typeof(WorkspaceView))]
    public class GroupMessagesWorkspace : MasterDetailsWorkspace, IGroupMessagesWorkspace
    {
        public override string DisplayName
        {
            get
            {
                return Resources.GroupMessage;
            }
        }

        public GroupMessagesWorkspace(IContactListViewModel defaultScreen, IDetailsConductorViewModel detailsConductor)
            : base(defaultScreen, detailsConductor)
        {
            defaultScreen.CanSelectMultipleItems = true;
        }
    }
}