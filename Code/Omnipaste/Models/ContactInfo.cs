namespace Omnipaste.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contacts.Models;

    public class ContactInfo : BaseModel
    {
        public const string NamePartSeparator = " ";

        private DateTime? _lastActivityTime;

        public ContactInfo()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            PhoneNumbers = new List<PhoneNumber>();
        }

        public ContactInfo(ContactDto contactDto)
            : this()
        {
            UniqueId = contactDto.Id;
            ContactId = contactDto.ContactId;
            FirstName = contactDto.FirstName;
            LastName = contactDto.LastName;
            Image = contactDto.Image;
            PhoneNumbers =
                contactDto.PhoneNumbers.Select(pn => new PhoneNumber { Number = pn.Number, Type = pn.Type }).ToList();
        }

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

        public DateTime? LastActivityTime
        {
            get
            {
                return _lastActivityTime;
            }
            set
            {
                _lastActivityTime = value.HasValue ? value.Value.ToUniversalTime() : (DateTime?)null;
            }
        }

        public override string ToString()
        {
            return string.Join(NamePartSeparator, Name, PhoneNumber);
        }

        public ContactInfo SetUniqueId(string uniqueId)
        {
            UniqueId = uniqueId;
            return this;
        }

        public ContactInfo SetLastActivityTime(DateTime? lastActivityTime)
        {
            LastActivityTime = lastActivityTime;
            return this;
        }

        public ContactInfo AddPhoneNumber(String phoneNumber)
        {
            PhoneNumbers.Add(new PhoneNumber(phoneNumber));
            return this;
        }

        public ContactInfo SetContactId(int? contactId)
        {
            ContactId = contactId ?? default(int);
            return this;
        }
    }
}