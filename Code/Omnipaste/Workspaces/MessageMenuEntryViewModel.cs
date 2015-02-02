namespace Omnipaste.Workspaces
{
    using OmniUI.Attributes;
    using OmniUI.MainMenuEntry;
    using OmniUI.Workspace;

    [UseView(typeof(MainMenuEntryView))]
    public class MessageMenuEntryViewModel : WorkspaceMainMenuEntry<IPeopleWorkspace>, IMainMenuEntryViewModel
    {
        public override string Icon
        {
            get
            {
                return OmniUI.Resources.IconNames.SideMenuMessages;
            }
        }
    }
}