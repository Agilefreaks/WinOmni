namespace Omnipaste.ContactList
{
    using Omnipaste.Services.Providers;
    using Omnipaste.Services.Repositories;

    public class ContactListViewModel : ContactListViewModelBase<IContactInfoViewModel>, IContactListViewModel
    {
        public ContactListViewModel(
            IContactRepository contactRepository,
            IContactInfoViewModelFactory contactInfoViewModelFactory)
            : base(contactRepository, contactInfoViewModelFactory)
        {
        }
    }
}