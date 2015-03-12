namespace Omnipaste.Workspaces
{
    using OmniUI.Workspace;

    public class GroupMessagesMainMenuEntryViewModel : WorkspaceMainMenuEntry<IGroupMessagesWorkspace>, IGroupMessagesMainMenuEntryViewModel
    {
        public override string Icon
        {
            get
            {
                return "appbar_new";
            }
        }
    }
}