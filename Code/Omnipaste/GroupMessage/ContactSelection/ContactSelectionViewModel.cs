namespace Omnipaste.GroupMessage.ContactSelection
{
    using Omnipaste.ContactList;
    using Omnipaste.Services.Providers;
    using Omnipaste.Services.Repositories;

    public class ContactSelectionViewModel : ContactListViewModelBase<IContactInfoViewModel>, IContactSelectionViewModel
    {
        public ContactSelectionViewModel(IContactRepository contactRepository, IConversationProvider conversationProvider, IContactInfoViewModelFactory contactInfoViewModelFactory)
            : base(contactRepository, conversationProvider, contactInfoViewModelFactory)
        {
        }
    }
}