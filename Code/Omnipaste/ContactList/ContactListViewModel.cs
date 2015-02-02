namespace Omnipaste.ContactList
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reactive.Linq;
    using OmniCommon.ExtensionMethods;
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
        protected override void OnActivate()
        {
            base.OnActivate();
            Subscriptions.Add(GetItemUpdatedObservable().SubscribeAndHandleErrors(UpdateViewModel));
        }

        private IObservable<ContactInfoPresenter> GetItemUpdatedObservable()
        {
            return _contactRepository.OperationObservable.Updated().Select(o => new ContactInfoPresenter(o.Item));
        }

        private void UpdateViewModel(ContactInfoPresenter obj)
        {
            var viewModel = Items.FirstOrDefault(vm => vm.Model.Identifier == obj.Identifier);
            if (viewModel != null)
            {
                viewModel.Model = obj;
            }
        }

        protected override IObservable<IEnumerable<ContactInfoPresenter>> GetFetchItemsObservable()
        {
            return
                _contactRepository.GetAll()
                    .Select(contacts => contacts.Select(contact => new ContactInfoPresenter(contact)));
        }

        protected override IObservable<ContactInfoPresenter> GetItemAddedObservable()
        {
            return _contactRepository.OperationObservable.Created()
                .Select(o => new ContactInfoPresenter(o.Item));
        }

        protected override IContactInfoViewModel CreateViewModel(ContactInfoPresenter model)
        {
            return _contactInfoViewModelFactory.Create(model);
        }

        private bool MatchesFilterText(IContactInfoPresenter model)
        {
            return (model != null)
                   && (string.IsNullOrWhiteSpace(FilterText)
                       || IsMatch(FilterText, model.ContactInfo.Name)
                       || IsMatch(FilterText, model.ContactInfo.Phone));
        }

        private bool MatchesFilter(IContactInfoPresenter model)
        {
            return !ShowStarred || model.IsStarred;
        }

        private bool IsMatch(string filter, string value)
        {
            return (CultureInfo.CurrentCulture.CompareInfo.IndexOf(value, filter, CompareOptions.IgnoreCase) > -1);
        }
    }
}