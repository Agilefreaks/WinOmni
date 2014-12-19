namespace OmniHolidays.MessagesWorkspace.ContactList
{
    using OmniUI.Models;
    using OmniUI.Presenters;

    public class ContactListContactInfoPresenter : ContactInfoPresenter
    {
        #region Constructors and Destructors

        public ContactListContactInfoPresenter()
        {
        }

        public ContactListContactInfoPresenter(IContactInfo contactInfo)
            : base(contactInfo)
        {
        }

        #endregion

        #region Methods

        protected override void UpdateContactIdentifier()
        {
            Identifier = string.Join(" ", ContactInfo.Name, ContactInfo.Phone);
        }

        #endregion
    }
}