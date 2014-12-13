namespace Omnipaste.Workspaces
{
    using OmniUI.Attributes;
    using OmniUI.MainMenuEntry;

    [UseView("OmniUI.MainMenuEntry.MainMenuEntryView", IsFullyQualifiedName = true)]
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
