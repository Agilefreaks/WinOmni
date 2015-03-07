namespace Omnipaste.Models
{
    using System;

    public class ContactInfo : BaseModel
    {
        public const string NamePartSeparator = " ";

        public ContactInfo()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Phone = string.Empty;
        }

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

        public bool IsStarred { get; set; }

        public override string ToString()
        {
            return string.Join(NamePartSeparator, Name, Phone);
        }
    }
}