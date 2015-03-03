namespace Omnipaste.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contacts.Models;

    public class ContactInfo : BaseModel
    {
        #region Constants

        public const string NamePartSeparator = " ";

        #endregion

        #region Constructors and Destructors

        public ContactInfo()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            PhoneNumbers = new List<PhoneNumber>();
        }

        public ContactInfo(ContactDto contactDto)
        {
            UniqueId = contactDto.Id;
            ContactId = contactDto.ContactId;
            FirstName = contactDto.FirstName;
            LastName = contactDto.LastName;
            Image = contactDto.Image;
            PhoneNumbers = contactDto.PhoneNumbers
                .Select(pn => new PhoneNumber { Number = pn.Number, Type = pn.Type })
                .ToList();
        }

        #endregion

        #region Public Properties

        public int ContactId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Image { get; set; }

        public Uri ImageUri { get; set; }

        public string Name
        {
            get
            {
                return string.Join(NamePartSeparator, FirstName, LastName);
            }
        }
        
        public string PhoneNumber
        {
            get
            {
                return PhoneNumbers.Any() ? PhoneNumbers[0].Number : string.Empty;
            }
        }

        public IList<PhoneNumber> PhoneNumbers { get; set; }

        public bool IsStarred { get; set; }

        #endregion

        public override string ToString()
        {
            return string.Join(NamePartSeparator, Name, PhoneNumber);
        }
    }
}