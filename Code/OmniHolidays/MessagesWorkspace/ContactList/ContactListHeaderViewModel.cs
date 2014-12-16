namespace OmniHolidays.MessagesWorkspace.ContactList
{
    using Caliburn.Micro;
    using OmniHolidays.Properties;

    public class ContactListHeaderViewModel : Screen, IContactListHeaderViewModel
    {
        public override string DisplayName
        {
            get
            {
                return Resources.ContactListHeader;
            }
        }
    }
}