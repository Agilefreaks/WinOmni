namespace Omnipaste.Workspaces
{
    using Omnipaste.GroupMessage;
    using OmniUI.Attributes;
    using OmniUI.Workspace;

    [UseView(typeof(WorkspaceView))]
    public class GroupMessagesWorkspace : Workspace, IGroupMessagesWorkspace
    {
        public override string DisplayName
        {
            get
            {
                return "+ New message";
            }
        }
        
        public GroupMessagesWorkspace(IGroupMessageViewModel defaultScreen)
            : base(defaultScreen)
        {
        }
    }
}