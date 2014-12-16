namespace OmniHolidays.MessagesWorkspace.ContactList
{
    using Caliburn.Micro;
    using Ninject;

    public class ContactListViewModel : Screen, IContactListViewModel
    {
        [Inject]
        public IContactListHeaderViewModel HeaderViewModel { get; set; }

        [Inject]
        public IContactListContentViewModel ContentViewModel { get; set; }
    }
}