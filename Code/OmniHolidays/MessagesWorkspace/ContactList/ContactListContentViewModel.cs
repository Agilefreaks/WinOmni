namespace OmniHolidays.MessagesWorkspace.ContactList
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using System.Reactive.Subjects;
    using Contacts.Models;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using OmniHolidays.Commands;
    using OmniUI.Framework;
    using OmniUI.List;
    using OmniUI.Models;
    using OmniUI.Presenters;
    using OmniUI.Services;

    public class ContactListContentViewModel : ListViewModelBase<IContactInfoPresenter, IContactViewModel>,
                                               IContactListContentViewModel
    {
        #region Fields

        private readonly ICommandService _commandService;

        private readonly Subject<IContactInfoPresenter> _contactInfoSubject;

        private readonly IKernel _kernel;

        private readonly IDeepObservableCollectionView _selectedContacts;

        private string _filterText;

        private bool _isBusy;

        private bool _selectAll;

        #endregion

        #region Constructors and Destructors

        public ContactListContentViewModel(ICommandService commandService, IKernel kernel)
            : base(new Subject<IContactInfoPresenter>())
        {
            _commandService = commandService;
            _kernel = kernel;
            _contactInfoSubject = EntityObservable as Subject<IContactInfoPresenter>;
            IsBusy = false;
            FilterText = string.Empty;
            ViewModelFilter = IdentifierContainsFilterText;
            FilteredItems.CustomSort = new ContactViewModelComparer();
            _selectedContacts = new DeepObservableCollectionView<IContactViewModel>(Items) { Filter = ModelIsSelected };
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

        protected override IContactViewModel CreateViewModel(IContactInfoPresenter entity)
        {
            var contactViewModel = _kernel.Get<IContactViewModel>();
            contactViewModel.Model = entity;

            return contactViewModel;
        }

        protected override bool MaxItemsLimitReached()
        {
            return false;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            if (Items.Count != 0)
            {
                return;
            }
            SyncContacts();
        }

        protected override void OnFilterUpdated()
        {
            base.OnFilterUpdated();
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

        private static bool ModelIsSelected(object viewModel)
        {
            var contactViewModel = viewModel as IContactViewModel;
            return contactViewModel != null && contactViewModel.IsSelected;
        }

        private bool IdentifierContainsFilterText(IContactViewModel viewModel)
        {
            return (viewModel != null)
                   && (string.IsNullOrWhiteSpace(FilterText)
                       || (CultureInfo.CurrentCulture.CompareInfo.IndexOf(
                           viewModel.Model.Identifier,
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
                _contactInfoSubject.OnNext(contactInfoPresenter);
            }

            IsBusy = false;
        }

        private void SyncContacts()
        {
            IsBusy = true;
            _commandService.Execute(new SyncContactsCommand()).Subscribe(OnGotNewContacts, OnGetContactsError);
        }

        private void SelectedContactsOnCollectionChanged(
            object sender,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            UpdateSelectAll();
        }

        private void UpdateContactSelection()
        {
            var selectAllChecked = SelectAll;
            foreach (IContactViewModel contactViewModel in FilteredItems)
            {
                contactViewModel.IsSelected = selectAllChecked;
            }
        }

        private void UpdateSelectAll()
        {
            _selectAll = FilteredItems.Count > 0
                         && FilteredItems.Cast<IContactViewModel>().All(viewModel => viewModel.IsSelected);
            NotifyOfPropertyChange(() => SelectAll);
        }

        #endregion
    }
}