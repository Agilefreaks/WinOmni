namespace Omnipaste.ContactList
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reactive.Linq;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Providers;
    using Omnipaste.Services.Repositories;
    using OmniUI.Details;
    using OmniUI.List;

    public abstract class ContactListViewModelBase<TContactViewModel> : ListViewModelBase<ContactInfoPresenter, TContactViewModel>, IContactListViewModel<TContactViewModel>
        where TContactViewModel : class, IDetailsViewModel<ContactInfoPresenter>
    {
        private readonly IConversationProvider _conversationProvider;

        private readonly IContactInfoViewModelFactory _contactInfoViewModelFactory;

        private bool _showStarred;

        private string _filterText;

        public IContactRepository ContactRepository { get; set; }

        protected ContactListViewModelBase(
            IContactRepository contactRepository,
            IConversationProvider conversationProvider,
            IContactInfoViewModelFactory contactInfoViewModelFactory)
        {
            ContactRepository = contactRepository;
            _conversationProvider = conversationProvider;
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

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Subscriptions.Add(
                GetItemUpdatedObservable()
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeAndHandleErrors(UpdateViewModel));

            Subscriptions.Add(_conversationProvider.All().ItemAdded
                .SubscribeOn(SchedulerProvider.Default)
                .ObserveOn(SchedulerProvider.Dispatcher)
                .SubscribeAndHandleErrors(OnConversationItemCreated));
        }

        protected override IObservable<IEnumerable<ContactInfoPresenter>> GetFetchItemsObservable()
        {
            return
                ContactRepository.GetAll()
                    .Select(contacts => contacts.Select(contact => new ContactInfoPresenter(contact)));
        }

        protected override IObservable<ContactInfoPresenter> GetItemAddedObservable()
        {
            return ContactRepository.OperationObservable.Created()
                .Select(o => new ContactInfoPresenter(o.Item));
        }

        protected override TContactViewModel CreateViewModel(ContactInfoPresenter model)
        {
            return _contactInfoViewModelFactory.Create<TContactViewModel>(model);
        }

        private static bool IsMatch(string filter, string value)
        {
            return (CultureInfo.CurrentCulture.CompareInfo.IndexOf(value, filter, CompareOptions.IgnoreCase) > -1);
        }

        private IObservable<ContactInfoPresenter> GetItemUpdatedObservable()
        {
            return ContactRepository.OperationObservable.Updated().Select(o => new ContactInfoPresenter(o.Item));
        }

        private ContactInfoPresenter GetContactFor(IConversationItem conversationItem)
        {
            return
                Items.Select(vm => vm.Model)
                    .FirstOrDefault(
                        model => conversationItem.ContactInfo.UniqueId == model.ContactInfo.UniqueId);
        }

        private void UpdateViewModel(ContactInfoPresenter obj)
        {
            var viewModel = Items.FirstOrDefault(vm => vm.Model.Identifier == obj.Identifier);
            if (viewModel != null)
            {
                viewModel.Model = obj;
            }
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

        private void OnConversationItemCreated(IConversationItem conversationItem)
        {
            var contactPresenter = GetContactFor(conversationItem);
            if (contactPresenter != null)
            {
                RefreshViewForItem(contactPresenter);
            }
        }
    }
}