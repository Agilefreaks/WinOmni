namespace Omnipaste.Models
{
    using System;
    using Events.Models;

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

        #endregion

        #region Public Properties

        public Uri ImageUri { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        #endregion
    }
}