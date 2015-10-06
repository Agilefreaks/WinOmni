namespace Omnipaste.Conversations.ContactList
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Controls;
    using Castle.Core.Internal;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.Conversations.ContactList.Contact;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.ExtensionMethods;
    using Omnipaste.Framework.Models;
    using Omnipaste.Framework.Services.Repositories;
    using OmniUI.Framework.ExtensionMethods;
    using OmniUI.List;
    using OmniUI.Workspaces;

    public class ContactListViewModel : ListViewModelBase<ContactModel, IContactViewModel>, IContactListViewModel
    {
        private readonly IContactViewModelFactory _contactViewModelFactory;

        private bool _canSelectMultipleItems;

        private string _filterText;

        private ContactModel _pendingContact;

        private bool _showStarred;

        private IMasterDetailsWorkspace _detailsWorkspace;

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

        public IContactRepository ContactRepository { get; set; }

        public ContactModel PendingContact
        {
            get
            {
                return _pendingContact ?? (_pendingContact = new ContactModel(new ContactEntity()));
            }
            set
            {
                if (_pendingContact == value)
                {
                    return;
                }

                _pendingContact = value;
                NotifyOfPropertyChange();
            }
        }

        public override int MaxItemCount
        {
            get
            {
                return 0;
            }
        }

        public bool IsRefreshing { get; set; }

        public ObservableCollection<ContactModel> SelectedContacts { get; private set; }

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

        public IMasterDetailsWorkspace DetailsWorkspace
        {
            get
            {
                return _detailsWorkspace ?? this.GetParentOfType<IMasterDetailsWorkspace>();
            }
            set
            {
                _detailsWorkspace = value;
            }
        }

        public override void RefreshItems()
        {
            base.RefreshItems();

            Status = FilteredItems.Count == 0 && !ShowStarred
                         ? ListViewModelStatusEnum.EmptyFilter
                         : ListViewModelStatusEnum.NotEmpty;
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

        public void SelectionChanged(SelectionChangedEventArgs args)
        {
            if (IsRefreshing)
            {
                return;
            }
            if (args.RemovedItems.Count != 0)
            {
                UnselectItems(args.RemovedItems.Cast<IContactViewModel>());
            }

            if (args.AddedItems.Count != 0)
            {
                if (!CanSelectMultipleItems)
                {
                    ClearItemSelection();
                }

                SelectItems(args.AddedItems.Cast<IContactViewModel>());
            }
        }

        public void AddPendingContact()
        {
            ContactRepository.Save(PendingContact.ContactEntity).SubscribeAndHandleErrors();
        }

        protected override bool CanShow(IContactViewModel viewModel)
        {
            var contactModel = viewModel.Model;
            return MatchesFilter(contactModel) && MatchesFilterText(contactModel);
        }

        protected override IObservable<ContactModel> GetFetchItemsObservable()
        {
            return
                ContactRepository.GetAll().SelectMany(contacts => contacts.Select(contact => new ContactModel(contact)));
        }

        protected override IObservable<ContactModel> GetItemChangedObservable()
        {
            return ContactRepository.GetOperationObservable().Changed().Select(o => new ContactModel(o.Item));
        }

        protected override IContactViewModel ChangeViewModel(ContactModel model)
        {
            var contactViewModel = UpdateViewModel(model) ?? _contactViewModelFactory.Create<IContactViewModel>(model);
            if (model.UniqueId == PendingContact.UniqueId)
            {
                if (!CanSelectMultipleItems)
                {
                    ClearItemSelection();
                }

                SelectItems(new[] { contactViewModel });
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
            var matchesFilterText = false;

            try
            {
                matchesFilterText = (model != null)
                                    && (string.IsNullOrWhiteSpace(FilterText)
                                        || IsMatch(FilterText, model.BackingEntity.Name)
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

        private void UnselectItems(IEnumerable<IContactViewModel> itemsToUnselect)
        {
            var contactInfoViewModels =
                itemsToUnselect.Where(i => SelectedContacts.Any(sc => i.Model.UniqueId == sc.UniqueId));
            IsRefreshing = true;
            foreach (var item in contactInfoViewModels)
            {
                var modelsToRemove = SelectedContacts.Where(c => c.UniqueId == item.Model.UniqueId).ToList();
                modelsToRemove.ForEach(m => SelectedContacts.Remove(m));
                item.IsSelected = false;
            }
            IsRefreshing = false;
        }

        private void SelectItems(IEnumerable<IContactViewModel> itemsToSelect)
        {
            IsRefreshing = true;
            foreach (var item in itemsToSelect.Where(i => SelectedContacts.All(sc => sc.UniqueId != i.Model.UniqueId)))
            {
                item.IsSelected = true;
                SelectedContacts.Add(item.Model);
            }
            IsRefreshing = false;
        }

        private void ClearItemSelection()
        {
            IsRefreshing = true;
            Items.Where(i => SelectedContacts.Any(c => c.UniqueId == i.Model.UniqueId)).ForEach(i => i.IsSelected = false);
            IsRefreshing = false;
            SelectedContacts.Clear();
        }
    }
}