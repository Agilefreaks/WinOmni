namespace OmniUI.Models
{
    using System;

    public class ContactInfo : IContactInfo
    {
        #region Constants

        public const string NamePartSeparator = " ";

        #endregion

        #region Constructors and Destructors

        public ContactInfo()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Phone = string.Empty;
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
                return string.Join(NamePartSeparator, FirstName, LastName);
            }
        }

        public string Phone { get; set; }

        #endregion

        public override string ToString()
        {
            return string.Join(NamePartSeparator, Name, Phone);
        }
    }
}