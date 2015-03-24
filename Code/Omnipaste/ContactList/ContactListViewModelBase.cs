namespace Omnipaste.ContactList
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reactive.Linq;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Repositories;
    using OmniUI.Details;
    using OmniUI.List;

    public abstract class ContactListViewModelBase<TContactViewModel> : ListViewModelBase<ContactInfoPresenter, TContactViewModel>, IContactListViewModel<TContactViewModel>
        where TContactViewModel : class, IDetailsViewModel<ContactInfoPresenter>
    {
        private readonly IContactInfoViewModelFactory _contactInfoViewModelFactory;

        private bool _showStarred;

        private string _filterText;

        public IContactRepository ContactRepository { get; set; }

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

        protected override bool CanShow(TContactViewModel viewModel)
        {
            var contactInfoPresenter = viewModel.Model;
            return MatchesFilter(contactInfoPresenter) && MatchesFilterText(contactInfoPresenter);
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
                   && (string.IsNullOrWhiteSpace(FilterText)
                       || IsMatch(FilterText, model.BackingModel.Name)
                       || model.BackingModel.PhoneNumbers.Any(pn => IsMatch(FilterText, pn.Number)));
        }

        private bool MatchesFilter(IContactInfoPresenter model)
        {
            return !ShowStarred || model.IsStarred;
        }

    }
}