namespace Omnipaste.Models
{
    using System;
    using System.Linq;
    using Events.Models;
    using OmniCommon.Models;
    using OmniUI.Models;
    using OmniUI.Presenters;

    public class ContactInfo : IContactInfo
    {
        #region Constants

        private const string NamePartSeparator = " ";

        #endregion

        #region Constructors and Destructors

        public ContactInfo()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Phone = string.Empty;
        }

        public ContactInfo(Event @event)
            : this()
        {
            var nameParts = string.IsNullOrWhiteSpace(@event.ContactName)
                                ? new string[0]
                                : @event.ContactName.Split(NamePartSeparator[0]);
            if (nameParts.Length == 1)
            {
                FirstName = nameParts.First();
            }
            else if (nameParts.Length > 1)
            {
                FirstName = string.Join(NamePartSeparator, nameParts.Take(nameParts.Length - 1));
                LastName = nameParts.Last();
            }

            Phone = @event.PhoneNumber;
        }

        public ContactInfo(UserInfo userInfo)
        {
            FirstName = userInfo.FirstName;
            LastName = userInfo.LastName;
            ImageUri = string.IsNullOrWhiteSpace(userInfo.ImageUrl) ? null : new Uri(userInfo.ImageUrl);
        }

        #endregion

        #region Public Properties

        public string FirstName { get; set; }

        public Uri ImageUri { get; set; }

        public string LastName { get; set; }

        public string Name
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }

        public string Phone { get; set; }

        #endregion
    }
}