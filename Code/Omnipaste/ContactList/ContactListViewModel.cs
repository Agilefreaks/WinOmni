namespace Omnipaste.ContactList
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Repositories;
    using OmniUI.List;

    public class ContactListViewModel : ListViewModelBase<ContactInfoPresenter, IContactInfoViewModel>, IContactListViewModel
    {
        private readonly IContactRepository _contactRepository;

        private readonly IContactInfoViewModelFactory _contactInfoViewModelFactory;

        private bool _showStarred;

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
    }
}