namespace OmniHolidays.MessagesWorkspace
{
    using Ninject;
    using OmniHolidays.MessagesWorkspace.ContactList;
    using OmniHolidays.MessagesWorkspace.MessageDetails;
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

        [Inject]
        public IMessageDetailsViewModel MessageDetailsViewModel { get; set; }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();
            DetailsConductor.ActivateItem(MessageDetailsViewModel);
        }

        #endregion
    }
}