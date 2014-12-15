namespace OmniHolidays.MessagesWorkspace
{
    using OmniUI.Attributes;
    using OmniUI.Workspace;

    [UseView("OmniUI.MainMenuEntry.MainMenuEntryView", IsFullyQualifiedName = true)]
    public class MessagesMainMenuEntryViewModel :  WorkspaceMainMenuEntry<IMessagesWorkspace>, IMessagesMainMenuEntryViewModel
    {
        public override string Icon
        {
            get
            {
                return "airplane_icon";
            }
        }
    }
}