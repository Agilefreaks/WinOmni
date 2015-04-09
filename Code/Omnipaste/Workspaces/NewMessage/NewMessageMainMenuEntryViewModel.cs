namespace Omnipaste.Workspaces.NewMessage
{
    using OmniUI.MainMenuEntry;
    using OmniUI.Workspace;

    public class NewMessageMainMenuEntryViewModel : WorkspaceMainMenuEntry<INewMessageWorkspace>, IMainMenuEntryViewModel
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