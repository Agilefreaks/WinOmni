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
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using OmniHolidays.Commands;
    using OmniUI.Framework;
    using OmniUI.Models;
    using OmniUI.Presenters;
    using OmniUI.Services;

    public class ContactListContentViewModel : Screen, IContactListContentViewModel
    {
        #region Constants

        private const int FetchContactsNormalDuration = 10;

        #endregion

        #region Fields

        private readonly ICommandService _commandService;

        private readonly BindableCollection<IContactInfoPresenter> _contactCollection;

        private readonly ListCollectionView _filteredItems;

        private readonly IDeepObservableCollectionView _selectedContacts;

        private IDisposable _contactsSubscription;

        private string _filterText;

        private bool _selectAll;

        private ContactListContentViewModelState _state;

        private IDisposable _syncSupervisorSubscription;

        #endregion

        #region Constructors and Destructors

        public ContactListContentViewModel(ICommandService commandService)
        {
            _commandService = commandService;
            _contactCollection = new BindableCollection<IContactInfoPresenter>();
            _filteredItems = (ListCollectionView)CollectionViewSource.GetDefaultView(_contactCollection);
            _filteredItems.Filter = IdentifierContainsFilterText;
            State = ContactListContentViewModelState.Normal;
            FilterText = string.Empty;
            FilteredItems.CustomSort = new ContactInfoPresenterComparer();
            _selectedContacts = new DeepObservableCollectionView<IContactInfoPresenter>(_contactCollection)
                                    {
                                        Filter =
                                            ModelIsSelected
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

        public ContactListContentViewModelState State
        {
            get
            {
                return _state;
            }
            set
            {
                if (value == _state)
                {
                    return;
                }
                _state = value;
                NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Public Methods and Operators

        public void RetryFetchContacts()
        {
            DisposeSyncSupervisorSubscription();
            DisposeContactsSubscription();
            SyncContacts();
        }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            State = ContactListContentViewModelState.Normal;
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
                DisposeSyncSupervisorSubscription();
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

        private void DisposeSyncSupervisorSubscription()
        {
            if (_syncSupervisorSubscription == null)
            {
                return;
            }
            _syncSupervisorSubscription.Dispose();
            _syncSupervisorSubscription = null;
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

        private void OnFetchTakingLong()
        {
            DisposeSyncSupervisorSubscription();
            State = ContactListContentViewModelState.FetchTakingLong;
        }

        private void OnGetContactsError(Exception exception)
        {
            DisposeSyncSupervisorSubscription();
            ExceptionReporter.Instance.Report(exception);
            State = ContactListContentViewModelState.FetchTakingLong;
        }

        private void OnGotNewContacts(ContactList contactList)
        {
            DisposeSyncSupervisorSubscription();
            foreach (var contactInfoPresenter in contactList.Contacts.SelectMany(GetContactInfoPresenter))
            {
                _contactCollection.Add(contactInfoPresenter);
            }

            State = ContactListContentViewModelState.Normal;
        }

        private void SelectedContactsOnCollectionChanged(
            object sender,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            UpdateSelectAll();
        }

        private void SyncContacts()
        {
            State = ContactListContentViewModelState.Busy;
            DisposeContactsSubscription();
            _contactsSubscription =
                _commandService.Execute(new SyncContactsCommand())
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .Subscribe(OnGotNewContacts, OnGetContactsError);
            _syncSupervisorSubscription =
                Observable.Timer(TimeSpan.FromSeconds(FetchContactsNormalDuration), SchedulerProvider.Default)
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(_ => OnFetchTakingLong());
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