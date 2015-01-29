namespace Omnipaste.Workspaces
{
    using Omnipaste.Conversation;
    using Omnipaste.Properties;
    using OmniUI.Attributes;
    using OmniUI.Workspace;

    [UseView(typeof(WorkspaceView))]

    public class MessageWorkspace : MasterDetailsWorkspace, IMessageWorkspace
    {
        public MessageWorkspace(IConversationListViewModel masterScreen, IDetailsConductorViewModel detailsConductor)
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