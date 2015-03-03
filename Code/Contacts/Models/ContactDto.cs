namespace Contacts.Models
{
    using System.Collections.Generic;

    public class ContactDto
    {
        public ContactDto()
        {
            PhoneNumbers = new List<PhoneNumberDto>();
        }

        public string Id { get; set; }

        public int ContactId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Image { get; set; }

        public List<PhoneNumberDto> PhoneNumbers { get; set; }
    }
}