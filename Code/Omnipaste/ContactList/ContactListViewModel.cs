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
    using Castle.Core.Internal;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.ContactList.Contact;
    using Omnipaste.Entities;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using Omnipaste.WorkspaceDetails;
    using OmniUI.ExtensionMethods;
    using OmniUI.List;
    using OmniUI.Workspace;

    public class ContactListViewModel : ListViewModelBase<ContactModel, IContactViewModel>, IContactListViewModel
    {
        private readonly IContactViewModelFactory _contactViewModelFactory;

        private bool _showStarred;

        private string _filterText;

        private bool _canSelectMultipleItems;
        
        private ContactModel _pendingContact;

        public IContactRepository ContactRepository { get; set; }

        [Inject]
        public IWorkspaceDetailsViewModelFactory DetailsViewModelFactory { get; set; }

        public ContactModel PendingContact
        {
            get
            {
                return _pendingContact
                       ?? (_pendingContact = new ContactModel(new ContactEntity()));
            }
            set
            {
                if (_pendingContact == value)
                {
                    return;
                }

                _pendingContact = value;
                NotifyOfPropertyChange(() => _pendingContact);
            }
        }

        public ContactListViewModel(
            IContactRepository contactRepository,
            IContactViewModelFactory contactViewModelFactory)
        {
            ContactRepository = contactRepository;
            _contactViewModelFactory = contactViewModelFactory;

            FilteredItems.SortDescriptions.Add(
                new SortDescription(
                    PropertyExtensions.GetPropertyName<IContactViewModel, DateTime?>(vm => vm.LastActivityTime),
                    ListSortDirection.Descending));
            
            FilteredItems.SortDescriptions.Add(
                new SortDescription(
                    PropertyExtensions.GetPropertyName<IContactViewModel, string>(vm => vm.Identifier),
                    ListSortDirection.Ascending));

            SelectedContacts = new ObservableCollection<ContactModel>();
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

                IsRefreshing = true;
                NotifyOfPropertyChange(() => FilterText);
                RefreshItems();
                IsRefreshing = false;
            }
        }

        public bool IsRefreshing { get; set; }

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

        private ObservableCollection<ContactModel> _selectedContacts;

        private IWorkspaceDetailsViewModel _detailsViewModel;

        public ObservableCollection<ContactModel> SelectedContacts
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

        protected override bool CanShow(IContactViewModel viewModel)
        {
            var contactModel = viewModel.Model;
            return MatchesFilter(contactModel) && MatchesFilterText(contactModel);
        }

        private void HideDetails()
        {
            var detailsConductorViewModel = this.GetParentOfType<IMasterDetailsWorkspace>().DetailsConductor;
            var activeItem = detailsConductorViewModel.ActiveItem;
            detailsConductorViewModel.DeactivateItem(activeItem, true);
        }

        private void ShowDetails()
        {
            _detailsViewModel = DetailsViewModelFactory.Create(SelectedContacts);

            this.GetParentOfType<IMasterDetailsWorkspace>().DetailsConductor.ActivateItem(_detailsViewModel);
        }

        protected override IObservable<ContactModel> GetFetchItemsObservable()
        {
            return
                ContactRepository.GetAll()
                    .SelectMany(contacts => contacts.Select(contact => new ContactModel(contact)));
        }

        protected override IObservable<ContactModel> GetItemChangedObservable()
        {
            return ContactRepository.GetOperationObservable().Changed()
                .Select(o => new ContactModel(o.Item));
        }

        protected override IContactViewModel ChangeViewModel(ContactModel model)
        {
            var contactViewModel = UpdateViewModel(model) ?? _contactViewModelFactory.Create<IContactViewModel>(model);

            if (model.Identifier == PendingContact.Identifier)
            {
                SelectedContacts.Add(contactViewModel.Model);

                PendingContact = null;
                FilterText = "";
            }

            return contactViewModel;
        }

        private IContactViewModel UpdateViewModel(ContactModel obj)
        {
            var viewModel = Items.FirstOrDefault(vm => vm.Model.ContactEntity.UniqueId == obj.ContactEntity.UniqueId);
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

        private bool MatchesFilterText(ContactModel model)
        {
            bool matchesFilterText = false;

            try
            {
                matchesFilterText = (model != null)
                                    && (string.IsNullOrWhiteSpace(FilterText) || IsMatch(FilterText, model.BackingEntity.Name)
                                        || model.BackingEntity.PhoneNumbers.Any(pn => IsMatch(FilterText, pn.Number)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return matchesFilterText;
        }

        private bool MatchesFilter(ContactModel model)
        {
            return !ShowStarred || model.IsStarred;
        }

        public void SelectionChanged(SelectionChangedEventArgs args)
        {
            if (IsRefreshing)
            {
                return;
            }
            if (args.RemovedItems.Count != 0)
            {
                args.RemovedItems.Cast<IContactViewModel>().ForEach(i => i.IsSelected = false);
                UnselectItems(args.RemovedItems.Cast<IContactViewModel>());
            }

            if (args.AddedItems.Count != 0)
            {
                if (!CanSelectMultipleItems)
                {
                    Items.Where(i => SelectedContacts.Any(c => c.UniqueId == i.Model.UniqueId)).ForEach(i => i.IsSelected = false);
                    SelectedContacts.Clear();
                }

                SelectItems(args.AddedItems.Cast<IContactViewModel>());
            }
        }

        public override void RefreshItems()
        {
            base.RefreshItems();

            Status = FilteredItems.Count == 0 && !ShowStarred ? ListViewModelStatusEnum.EmptyFilter : ListViewModelStatusEnum.NotEmpty;
            NotifyOfPropertyChange(() => SelectedContacts);
        }

        public override void NotifyOfPropertyChange(string propertyName = null)
        {
            base.NotifyOfPropertyChange(propertyName);

            if (propertyName == "FilterText")
            {
                var phoneNumber = Regex.Replace(FilterText, "[^+0-9]", "");
                PendingContact.PhoneNumber = phoneNumber;
            }
        }

        public void AddPendingContact()
        {
            ContactRepository.Save(PendingContact.ContactEntity).SubscribeAndHandleErrors();
        }

        private void UnselectItems(IEnumerable<IContactViewModel> itemsToUnselect)
        {
            var contactModels =
                SelectedContacts.Where(sc => itemsToUnselect.Any(i => i.Model.UniqueId == sc.UniqueId)).ToList();
            foreach (var item in contactModels)
            {
                SelectedContacts.Remove(item);
            }
        }

        private void SelectItems(IEnumerable<IContactViewModel> itemsToSelect)
        {
            foreach (var item in itemsToSelect.Where(i => SelectedContacts.All(sc => sc.UniqueId != i.Model.UniqueId)))
            {
                SelectedContacts.Add(item.Model);
            }
        }
    }
}