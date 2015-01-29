namespace Omnipaste.Workspaces
{
    using Omnipaste.ContactList;
    using Omnipaste.Properties;
    using OmniUI.Attributes;
    using OmniUI.Workspace;

    [UseView(typeof(WorkspaceView))]

    public class MessageWorkspace : MasterDetailsWorkspace, IMessageWorkspace
    {
        public MessageWorkspace(IContactListViewModel masterScreen, IDetailsConductorViewModel detailsConductor)
            : base(masterScreen, detailsConductor)
        {
        }

        public override string DisplayName
        {
            get
            {
                return Resources.Messages;
            }
        }
    }
}