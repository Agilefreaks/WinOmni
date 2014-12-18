namespace OmniHolidays.MessagesWorkspace.ContactList
{
    using Caliburn.Micro;
    using OmniHolidays.MessagesWorkspace.MessageDetails;
    using OmniUI.ExtensionMethods;

    public class ContactListViewModel : Screen, IContactListViewModel
    {
        private readonly IContactListHeaderViewModel _header;

        private readonly IContactListContentViewModel _content;

        private readonly IMessageDetailsViewModel _details;

        #region Public Properties

        public ContactListViewModel(IContactListHeaderViewModel header, IContactListContentViewModel content, IMessageDetailsViewModel details)
        {
            _header = header;
            _content = content;
            _details = details;
        }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();
            _header.Activate();
            _content.Activate();
            _details.ContactsSource = _content;
            this.GetParentOfType<IMessagesWorkspace>().DetailsConductor.ActivateItem(_details);
        }

        protected override void OnDeactivate(bool close)
        {
            _header.Deactivate(close);
            _content.Deactivate(close);
            _details.Deactivate(close);
            base.OnDeactivate(close);
        }

        #endregion
    }
}