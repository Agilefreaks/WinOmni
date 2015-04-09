namespace Omnipaste.Workspaces.Clippings
{
    using OmniUI.Attributes;
    using OmniUI.MainMenuEntry;
    using OmniUI.Workspace;

    [UseView(typeof(MainMenuEntryView))]
    public class ClippingsMenuEntryViewModel : WorkspaceMainMenuEntry<IClippingsWorkspace>, IMainMenuEntryViewModel
    {
        public override string Icon
        {
            get
            {
                return OmniUI.Resources.IconNames.SideMenuClippings;
            }
        }
    }
}
