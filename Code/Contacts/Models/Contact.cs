namespace Contacts.Models
{
    using System.Collections.Generic;

    public class Contact
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<ContactPhoneNumber> Numbers { get; set; }
    }
}
