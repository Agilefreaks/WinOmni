namespace OmniHolidays.MessagesWorkspace.ContactList
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Contacts.Models;
    using OmniUI.Models;
    using OmniUI.Presenters;

    public class ContactListContactInfoPresenter : ContactInfoPresenter
    {
        #region Fields

        private string _imageData;

        #endregion

        #region Constructors and Destructors

        private ContactListContactInfoPresenter(IContactInfo contactInfo)
            : base(contactInfo)
        {
        }

        #endregion

        #region Public Methods and Operators

        public static IList<ContactListContactInfoPresenter> CreateFromContact(Contact contact)
        {
            return
                contact.Numbers.Select(
                    contactPhoneNumber =>
                    new ContactListContactInfoPresenter(
                        new ContactInfo
                            {
                                FirstName = contact.FirstName,
                                LastName = contact.LastName,
                                Phone = contactPhoneNumber.Number,
                            }) { _imageData = contact.Photo }).ToList();
        }

        #endregion

        #region Methods

        protected override ImageSource GetContactImage()
        {
            return string.IsNullOrWhiteSpace(_imageData) ? base.GetContactImage() : GetImageSourceFromBase64();
        }

        protected override string GetIdentifier()
        {
            return string.Join(" ", ContactInfo.Name, ContactInfo.Phone);
        }

        private ImageSource GetImageSourceFromBase64()
        {
            var imageBytes = Convert.FromBase64String(_imageData);
            using (var stream = new MemoryStream(imageBytes))
            {
                return BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
        }

        #endregion
    }
}