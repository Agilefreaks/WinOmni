namespace Omnipaste.Models
{
    using System;
    using Events.Models;
    using OmniCommon.Models;

    public class ContactInfo
    {
        #region Constructors and Destructors

        public ContactInfo()
        {
            Name = string.Empty;
            Phone = string.Empty;
        }

        public ContactInfo(Event @event)
        {
            Name = @event.ContactName;
            Phone = @event.PhoneNumber;
        }

        public ContactInfo(UserInfo userInfo)
        {
            Name = userInfo.FullName();
            ImageUri = string.IsNullOrWhiteSpace(userInfo.ImageUrl) ? null : new Uri(userInfo.ImageUrl);
        }

        #endregion

        #region Public Properties

        public Uri ImageUri { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        #endregion
    }
}