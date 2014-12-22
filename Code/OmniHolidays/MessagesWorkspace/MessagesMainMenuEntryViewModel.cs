namespace OmniHolidays.MessagesWorkspace
{
    using OmniUI.Workspace;

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