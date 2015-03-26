namespace Omnipaste.ContactList
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Controls;
    using Ninject;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Repositories;
    using Omnipaste.WorkspaceDetails;
    using OmniUI.Details;
    using OmniUI.ExtensionMethods;
    using OmniUI.List;
    using OmniUI.Workspace;

    public abstract class ContactListViewModelBase<TContactViewModel> : ListViewModelBase<ContactInfoPresenter, TContactViewModel>, IContactListViewModel<TContactViewModel>
        where TContactViewModel : class, IDetailsViewModel<ContactInfoPresenter>
    {
        private readonly IContactInfoViewModelFactory _contactInfoViewModelFactory;

        private bool _showStarred;

        private string _filterText;

        private bool _canSelectMultipleItems = false;
        
        private ContactInfoPresenter _pendingContact;

        public IContactRepository ContactRepository { get; set; }



        [Inject]
        public IWorkspaceDetailsViewModelFactory DetailsViewModelFactory { get; set; }

        public ContactInfoPresenter PendingContact
        {
            get
            {
                return _pendingContact
                       ?? (_pendingContact = new ContactInfoPresenter(new Models.ContactInfo()));
            }
            set
            {
                if (_pendingContact != value)
                {
                    _pendingContact = value;
                    NotifyOfPropertyChange(() => _pendingContact);
                }
            }
        }


        protected ContactListViewModelBase(
            IContactRepository contactRepository,
            IContactInfoViewModelFactory contactInfoViewModelFactory)
        {
            ContactRepository = contactRepository;
            _contactInfoViewModelFactory = contactInfoViewModelFactory;

            FilteredItems.SortDescriptions.Add(
                new SortDescription(
                    PropertyExtensions.GetPropertyName<IContactInfoViewModel, DateTime?>(vm => vm.LastActivityTime),
                    ListSortDirection.Ascending));

            SelectedContacts = new ObservableCollection<ContactInfoPresenter>();
        }

        public override int MaxItemCount
        {
            get
            {
                return 0;
            }
        }

        public bool ShowStarred
        {
            get
            {
                return _showStarred;
            }
            set
            {
                if (value.Equals(_showStarred))
                {
                    return;
                }
                _showStarred = value;
                NotifyOfPropertyChange(() => ShowStarred);
                RefreshItems();
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
                
                NotifyOfPropertyChange(() => FilterText);
                
                RefreshItems();
            }
        }

        public bool CanSelectMultipleItems
        {
            get
            {
                return _canSelectMultipleItems;
            }
            set
            {
                if (value.Equals(_canSelectMultipleItems))
                {
                    return;
                }

                _canSelectMultipleItems = value;
                NotifyOfPropertyChange(() => CanSelectMultipleItems);
            }
        }

        private ObservableCollection<ContactInfoPresenter> _selectedContacts;

        private IWorkspaceDetailsViewModel _detailsViewModel;

        public ObservableCollection<ContactInfoPresenter> SelectedContacts
        {
            get
            {
                return _selectedContacts;
            }
            set
            {
                if (_selectedContacts == value)
                {
                    return;
                }

                _selectedContacts = value;
                _selectedContacts.CollectionChanged += SelectedContactsCollectionChanged;
                NotifyOfPropertyChange(() => SelectedContacts);
            }
        }

        public void SelectedContactsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
                if (CanSelectMultipleItems)
                {
                    if (e.Action == NotifyCollectionChangedAction.Add && SelectedContacts.Count == 1)
                    {
                        ShowDetails();
                    }
                    else if (e.Action == NotifyCollectionChangedAction.Remove && SelectedContacts.Count == 0)
                    {
                        HideDetails();
                    }
                }
                else
                {
                    if (SelectedContacts.Count == 1)
                    {
                        ShowDetails();
                    }
                }
                
        }

        protected override bool CanShow(TContactViewModel viewModel)
        {
            var contactInfoPresenter = viewModel.Model;
            return MatchesFilter(contactInfoPresenter) && MatchesFilterText(contactInfoPresenter);
        }

        private void HideDetails()
        {
            var detailsConductorViewModel = this.GetParentOfType<IMasterDetailsWorkspace>().DetailsConductor;
            var activeItem = detailsConductorViewModel.ActiveItem;
            detailsConductorViewModel.DeactivateItem(activeItem, true);
        }

        private void ShowDetails()
        {
            _detailsViewModel = CanSelectMultipleItems
                                    ? DetailsViewModelFactory.Create(SelectedContacts)
                                    : DetailsViewModelFactory.Create(SelectedContacts.First());

            this.GetParentOfType<IMasterDetailsWorkspace>().DetailsConductor.ActivateItem(_detailsViewModel);
        }

        protected override IObservable<ContactInfoPresenter> GetFetchItemsObservable()
        {
            return
                ContactRepository.GetAll()
                    .SelectMany(contacts => contacts.Select(contact => new ContactInfoPresenter(contact)));
        }

        protected override IObservable<ContactInfoPresenter> GetItemChangedObservable()
        {
            return ContactRepository.GetOperationObservable().Changed()
                .Select(o => new ContactInfoPresenter(o.Item));
        }

        protected override TContactViewModel ChangeViewModel(ContactInfoPresenter model)
        {
            var contactInfoViewModel = UpdateViewModel(model) ?? _contactInfoViewModelFactory.Create<TContactViewModel>(model);
            return contactInfoViewModel;
        }

        private TContactViewModel UpdateViewModel(ContactInfoPresenter obj)
        {
            var viewModel = Items.FirstOrDefault(vm => vm.Model.ContactInfo.UniqueId == obj.ContactInfo.UniqueId);
            if (viewModel != null)
            {
                viewModel.Model = obj;
                FilteredItems.EditItem(viewModel);
                FilteredItems.CommitEdit();
            }

            return viewModel;
        }

        private static bool IsMatch(string filter, string value)
        {
            return (CultureInfo.CurrentCulture.CompareInfo.IndexOf(value, filter, CompareOptions.IgnoreCase) > -1);
        }

        private IObservable<ContactInfoPresenter> GetItemUpdatedObservable()
        {
            return ContactRepository.GetOperationObservable().Changed().Select(o => new ContactInfoPresenter(o.Item));
        }

        private bool MatchesFilterText(IContactInfoPresenter model)
        {

            return (model != null)
                   && (string.IsNullOrWhiteSpace(FilterText) || IsMatch(FilterText, model.BackingModel.Name)
                       || model.BackingModel.PhoneNumbers.Any(pn => IsMatch(FilterText, pn.Number)));
        }

        private bool MatchesFilter(IContactInfoPresenter model)
        {
            return !ShowStarred || model.IsStarred;
        }

        public void SelectionChanged(SelectionChangedEventArgs args)
        {
            if (args.RemovedItems.Count != 0)
            {
                UnselectItems(args.RemovedItems.Cast<IContactInfoViewModel>().Where(vm => FilteredItems.Contains(vm)));
            }

            if (args.AddedItems.Count != 0)
            {
                SelectItems(args.AddedItems.Cast<IContactInfoViewModel>());
            }
        }

        public override void RefreshItems()
        {
            base.RefreshItems();

            NotifyOfPropertyChange(() => SelectedContacts);
        }

        public override void NotifyOfPropertyChange(string propertyName)
        {
            base.NotifyOfPropertyChange(propertyName);

            if (propertyName == "FilterText")
            {
                var phoneNumber = Regex.Replace(FilterText, "[^+0-9]", "");
                PendingContact.PhoneNumber = phoneNumber;
            }
        }

        private void AddedPendingContact(TContactViewModel item)
        {
            _pendingContact = new ContactInfoPresenter(new Models.ContactInfo());
            SelectedContacts.Add(item.Model);
            NotifyOfPropertyChange(() => PendingContact);
            FilterText = "";
        }

        public void ActivateItem(TContactViewModel item)
        {
            base.ActivateItem(item);

            if (item.Model.Identifier == PendingContact.Identifier)
            {
                AddedPendingContact(item);
            }
        }

        public void AddPendingContact()
        {
            ContactRepository.Save(PendingContact.ContactInfo).Subscribe();
        }

        private void UnselectItems(IEnumerable<IContactInfoViewModel> itemsToUnselect)
        {
            foreach (var item in itemsToUnselect)
            {
                SelectedContacts.Remove(item.Model);
            }
        }

        private void SelectItems(IEnumerable<IContactInfoViewModel> itemsToSelect)
        {
            foreach (var item in itemsToSelect.Where(i => !SelectedContacts.Contains(i.Model)))
            {
                SelectedContacts.Add(item.Model);
            }
        }
    }
}