namespace Omnipaste.Workspaces
{
    using OmniUI.Workspace;

    public class NewMessageMainMenuEntryViewModel : WorkspaceMainMenuEntry<INewMessageWorkspace>, INewMessageMainMenuEntryViewModel
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