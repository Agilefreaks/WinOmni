namespace Omnipaste.Workspaces.Activity
{
    using OmniUI.Attributes;
    using OmniUI.MainMenuEntry;
    using OmniUI.Workspace;

    [UseView(typeof(MainMenuEntryView))]
    public class ActivityMenuEntryViewModel : WorkspaceMainMenuEntry<IActivityWorkspace>, IMainMenuEntryViewModel
    {
        public override string Icon
        {
            get
            {
                return OmniUI.Resources.IconNames.SideMenuActivity;
            }
        }
    }
}
