namespace OmniHolidays.MessagesWorkspace.MessageDetails
{
    using OmniHolidays.MessagesWorkspace.ContactList;
    using OmniUI.Attributes;
    using OmniUI.Details;

    [UseView(typeof(DetailsViewWithHeader))]
    public class MessageDetailsViewModel :
        DetailsViewModelWithHeaderBase<IMessageDetailsHeaderViewModel, IMessageDetailsContentViewModel>,
        IMessageDetailsViewModel
    {
        #region Fields

        private IContactSource _contactsSource;

        #endregion

        #region Constructors and Destructors

        public MessageDetailsViewModel(
            IMessageDetailsHeaderViewModel headerViewModel,
            IMessageDetailsContentViewModel contentViewModel)
            : base(headerViewModel, contentViewModel)
        {
        }

        #endregion

        #region Public Properties

        public IContactSource ContactsSource
        {
            get
            {
                return _contactsSource;
            }
            set
            {
                if (Equals(value, _contactsSource))
                {
                    return;
                }
                _contactsSource = value;
                NotifyOfPropertyChange();
                HeaderViewModel.ContactsSource = ContactsSource;
                ContentViewModel.ContactSource = ContactsSource;
            }
        }

        public void Reset()
        {
            HeaderViewModel.Reset();
            ContentViewModel.Reset();
        }

        #endregion
    }
}