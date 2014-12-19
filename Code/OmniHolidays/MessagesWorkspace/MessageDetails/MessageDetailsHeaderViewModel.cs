namespace OmniHolidays.MessagesWorkspace.MessageDetails
{
    using System.Collections.Specialized;
    using System.Linq;
    using Caliburn.Micro;
    using OmniHolidays.MessagesWorkspace.ContactList;

    public class MessageDetailsHeaderViewModel : Screen, IMessageDetailsHeaderViewModel
    {
        #region Fields

        private IContactSource _contactSource;

        #endregion

        #region Public Properties

        public IContactSource ContactsSource
        {
            get
            {
                return _contactSource;
            }
            set
            {
                if (Equals(value, _contactSource))
                {
                    return;
                }
                UpdateContactSourceHooks(_contactSource, value);
                _contactSource = value;
                NotifyOfPropertyChange();
                UpdateDisplayName();
            }
        }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            UpdateDisplayName();
            UpdateContactSourceHooks(null, ContactsSource);
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            UpdateContactSourceHooks(ContactsSource, null);

            base.OnDeactivate(close);
        }

        private void SelectedContactsOnCollectionChanged(
            object sender,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            UpdateDisplayName();
        }

        private void UpdateContactSourceHooks(IContactSource currentSource, IContactSource newSource)
        {
            if (currentSource != null)
            {
                currentSource.Contacts.CollectionChanged -= SelectedContactsOnCollectionChanged;
            }

            if (newSource != null)
            {
                newSource.Contacts.CollectionChanged += SelectedContactsOnCollectionChanged;
            }
        }

        private void UpdateDisplayName()
        {
            DisplayName =
                ContactsSource.Contacts.Cast<IContactViewModel>()
                    .Select(viewModel => viewModel.Model.Identifier)
                    .Aggregate(string.Empty, (current, next) => string.Join(" ", current, next));
        }

        #endregion
    }
}