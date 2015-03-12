namespace Omnipaste.Workspaces
{
    using OmniUI.Attributes;
    using OmniUI.MainMenuEntry;
    using OmniUI.Workspace;

    [UseView(typeof(MainMenuEntryView))]
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