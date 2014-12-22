namespace OmniHolidays.MessagesWorkspace
{
    using OmniHolidays.MessagesWorkspace.ContactList;
    using OmniHolidays.Properties;
    using OmniUI.Attributes;
    using OmniUI.Workspace;

    [UseView(typeof(WorkspaceView))]
    public class MessagesWorkspace : MasterDetailsWorkspace, IMessagesWorkspace
    {
        #region Constructors and Destructors

        public MessagesWorkspace(IContactListViewModel masterScreen, IDetailsConductorViewModel detailsConductor)
            : base(masterScreen, detailsConductor)
        {
        }

        #endregion

        #region Public Properties

        public override string DisplayName
        {
            get
            {
                return Resources.Holidays;
            }
        }

        public override string Background
        {
            get
            {
                return "HolidayWorkspaceBackgroundBrush";
            }
        }

        #endregion
    }
}