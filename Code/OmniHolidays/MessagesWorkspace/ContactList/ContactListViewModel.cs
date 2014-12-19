namespace OmniHolidays.MessagesWorkspace.ContactList
{
    using Caliburn.Micro;
    using OmniHolidays.MessagesWorkspace.MessageDetails;
    using OmniUI.ExtensionMethods;

    public class ContactListViewModel : Screen, IContactListViewModel
    {
        #region Fields

        private readonly IContactListContentViewModel _contentViewModel;

        private readonly IMessageDetailsViewModel _details;

        private readonly IContactListHeaderViewModel _headerViewModel;

        #endregion

        #region Constructors and Destructors

        public ContactListViewModel(
            IContactListHeaderViewModel headerViewModel,
            IContactListContentViewModel contentViewModel,
            IMessageDetailsViewModel details)
        {
            _headerViewModel = headerViewModel;
            _contentViewModel = contentViewModel;
            _details = details;
        }

        #endregion

        #region Public Properties

        public IContactListContentViewModel ContentViewModel
        {
            get
            {
                return _contentViewModel;
            }
        }

        public IContactListHeaderViewModel HeaderViewModel
        {
            get
            {
                return _headerViewModel;
            }
        }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();
            HeaderViewModel.Activate();
            ContentViewModel.Activate();
            _details.ContactsSource = ContentViewModel;
            this.GetParentOfType<IMessagesWorkspace>().DetailsConductor.ActivateItem(_details);
        }

        protected override void OnDeactivate(bool close)
        {
            HeaderViewModel.Deactivate(close);
            ContentViewModel.Deactivate(close);
            _details.Deactivate(close);
            base.OnDeactivate(close);
        }

        #endregion
    }
}