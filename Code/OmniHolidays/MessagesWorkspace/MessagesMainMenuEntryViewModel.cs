namespace OmniHolidays.MessagesWorkspace
{
    using OmniUI.Attributes;
    using OmniUI.Workspace;

    [UseView(typeof(OmniUI.MainMenuEntry.MainMenuEntryView))]
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