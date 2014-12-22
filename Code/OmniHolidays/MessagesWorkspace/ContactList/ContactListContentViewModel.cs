namespace OmniHolidays.MessagesWorkspace.ContactList
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Windows.Data;
    using Caliburn.Micro;
    using Contacts.Models;
    using OmniCommon.Helpers;
    using OmniHolidays.Commands;
    using OmniUI.Framework;
    using OmniUI.Models;
    using OmniUI.Presenters;
    using OmniUI.Services;

    public class ContactListContentViewModel : Screen, IContactListContentViewModel
    {
        #region Fields

        private readonly ICommandService _commandService;

        private readonly BindableCollection<IContactInfoPresenter> _contactCollection;

        private readonly ListCollectionView _filteredItems;

        private readonly IDeepObservableCollectionView _selectedContacts;

        private IDisposable _contactsSubscription;

        private string _filterText;

        private bool _isBusy;

        private bool _selectAll;

        #endregion

        #region Constructors and Destructors

        public ContactListContentViewModel(ICommandService commandService)
        {
            _commandService = commandService;
            _contactCollection = new BindableCollection<IContactInfoPresenter>();
            _filteredItems = (ListCollectionView)CollectionViewSource.GetDefaultView(_contactCollection);
            _filteredItems.Filter = IdentifierContainsFilterText;
            IsBusy = false;
            FilterText = string.Empty;
            FilteredItems.CustomSort = new ContactInfoPresenterComparer();
            _selectedContacts = new DeepObservableCollectionView<IContactInfoPresenter>(_contactCollection)
                                    {
                                        Filter = ModelIsSelected
                                    };
            _selectedContacts.CollectionChanged += SelectedContactsOnCollectionChanged;
        }

        #endregion

        #region Public Properties

        public IDeepObservableCollectionView Contacts
        {
            get
            {
                return _selectedContacts;
            }
        }

        public string FilterText
        {
            get
            {
                return _filterText;
            }
            set
            {
                if (value == _filterText)
                {
                    return;
                }
                _filterText = value;
                NotifyOfPropertyChange();
                OnFilterUpdated();
            }
        }

        public ListCollectionView FilteredItems
        {
            get
            {
                return _filteredItems;
            }
        }

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                if (value.Equals(_isBusy))
                {
                    return;
                }
                _isBusy = value;
                NotifyOfPropertyChange();
            }
        }

        public IObservableCollection<IContactInfoPresenter> Items
        {
            get
            {
                return _contactCollection;
            }
        }

        public bool SelectAll
        {
            get
            {
                return _selectAll;
            }
            set
            {
                if (value.Equals(_selectAll))
                {
                    return;
                }
                _selectAll = value;
                NotifyOfPropertyChange();
                UpdateContactSelection();
            }
        }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();
            if (_contactCollection.Count != 0)
            {
                return;
            }
            SyncContacts();
        }

        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                DisposeContactsSubscription();
            }

            base.OnDeactivate(close);
        }

        protected void OnFilterUpdated()
        {
            _filteredItems.Refresh();
            UpdateSelectAll();
        }

        private static IList<ContactListContactInfoPresenter> GetContactInfoPresenter(Contact contact)
        {
            return
                contact.Numbers.Select(
                    phoneNumber =>
                    new ContactListContactInfoPresenter(
                        new ContactInfo
                            {
                                FirstName = contact.FirstName,
                                LastName = contact.LastName,
                                Phone = phoneNumber.Number
                            })).ToList();
        }

        private static bool ModelIsSelected(object contact)
        {
            var contactInfoPresenter = contact as IContactInfoPresenter;
            return contactInfoPresenter != null && contactInfoPresenter.IsSelected;
        }

        private void DisposeContactsSubscription()
        {
            if (_contactsSubscription == null)
            {
                return;
            }
            _contactsSubscription.Dispose();
            _contactsSubscription = null;
        }

        private bool IdentifierContainsFilterText(object item)
        {
            var model = item as IContactInfoPresenter;
            return (model != null)
                   && (string.IsNullOrWhiteSpace(FilterText)
                       || (CultureInfo.CurrentCulture.CompareInfo.IndexOf(
                           model.Identifier,
                           FilterText,
                           CompareOptions.IgnoreCase) > -1));
        }

        private void OnGetContactsError(Exception exception)
        {
            ExceptionReporter.Instance.Report(exception);
            IsBusy = false;
        }

        private void OnGotNewContacts(ContactList contactList)
        {
            foreach (var contactInfoPresenter in contactList.Contacts.SelectMany(GetContactInfoPresenter))
            {
                _contactCollection.Add(contactInfoPresenter);
            }

            IsBusy = false;
        }

        private void SelectedContactsOnCollectionChanged(
            object sender,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            UpdateSelectAll();
        }

        private void SyncContacts()
        {
            IsBusy = true;
            DisposeContactsSubscription();
            _contactsSubscription =
                _commandService.Execute(new SyncContactsCommand())
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .Subscribe(OnGotNewContacts, OnGetContactsError);
        }

        private void UpdateContactSelection()
        {
            var selectAllChecked = SelectAll;
            foreach (IContactInfoPresenter contact in FilteredItems)
            {
                contact.IsSelected = selectAllChecked;
            }
        }

        private void UpdateSelectAll()
        {
            _selectAll = FilteredItems.Count > 0
                         && FilteredItems.Cast<IContactInfoPresenter>().All(contact => contact.IsSelected);
            NotifyOfPropertyChange(() => SelectAll);
        }

        #endregion
    }
}