namespace Omnipaste.ContactList
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reactive.Linq;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Repositories;
    using OmniUI.List;

    public class ContactListViewModel : ListViewModelBase<ContactInfoPresenter, IContactInfoViewModel>, IContactListViewModel
    {
        private readonly IContactRepository _contactRepository;

        private readonly IContactInfoViewModelFactory _contactInfoViewModelFactory;

        private bool _showStarred;

        private string _filterText;

        public ContactListViewModel(IContactRepository contactRepository, IContactInfoViewModelFactory contactInfoViewModelFactory)
        {
            _contactRepository = contactRepository;
            _contactInfoViewModelFactory = contactInfoViewModelFactory;

            FilteredItems.SortDescriptions.Add(new SortDescription(PropertyExtensions.GetPropertyName<IContactInfoViewModel, DateTime?>(vm => vm.LastActivityTime), ListSortDirection.Descending));
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
                NotifyOfPropertyChange();
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

        protected override bool CanShow(IContactInfoViewModel viewModel)
        {
            var contactInfoPresenter = viewModel.Model;
            return MatchesFilter(contactInfoPresenter) && MatchesFilterText(contactInfoPresenter);
        }

        protected override IObservable<IEnumerable<ContactInfoPresenter>> GetFetchItemsObservable()
        {
            return
                _contactRepository.GetAll()
                    .Select(contacts => contacts.Select(contact => new ContactInfoPresenter(contact)));
        }

        protected override IObservable<ContactInfoPresenter> GetItemChangedObservable()
        {
            return _contactRepository.OperationObservable.Changed()
                .Select(o => new ContactInfoPresenter(o.Item));
        }

        protected override IContactInfoViewModel ChangeViewModel(ContactInfoPresenter model)
        {
            var contactInfoViewModel = UpdateViewModel(model) ?? _contactInfoViewModelFactory.Create(model);
            return contactInfoViewModel;
        }

        private static bool IsMatch(string filter, string value)
        {
            return (CultureInfo.CurrentCulture.CompareInfo.IndexOf(value, filter, CompareOptions.IgnoreCase) > -1);
        }

        private IContactInfoViewModel UpdateViewModel(ContactInfoPresenter obj)
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

        private bool MatchesFilterText(IContactInfoPresenter model)
        {
            return (model != null)
                   && (string.IsNullOrWhiteSpace(FilterText)
                       || IsMatch(FilterText, model.ContactInfo.Name)
                       || model.ContactInfo.PhoneNumbers.Any(pn => IsMatch(FilterText, pn.Number)));
        }

        private bool MatchesFilter(IContactInfoPresenter model)
        {
            return !ShowStarred || model.IsStarred;
        }
    }
}