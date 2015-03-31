namespace Omnipaste.Workspaces
{
    using Omnipaste.ContactList;
    using Omnipaste.Properties;
    using OmniUI.Attributes;
    using OmniUI.Workspace;

    [UseView(typeof(WorkspaceView))]
    public class NewMessageWorkspace : MasterDetailsWorkspace, INewMessageWorkspace
    {
        public override string DisplayName
        {
            get
            {
                return Resources.GroupMessage;
            }
        }

        public NewMessageWorkspace(IContactListViewModel defaultScreen, IDetailsConductorViewModel detailsConductor)
            : base(defaultScreen, detailsConductor)
        {
            defaultScreen.CanSelectMultipleItems = true;
        }
    }
}