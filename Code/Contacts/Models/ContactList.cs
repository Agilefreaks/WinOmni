namespace Contacts.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using OmniApi.Support.Converters;

    public class ContactList
    {
        public ContactList()
        {
            Contacts = new List<ContactDto>();
        }

        [JsonConverter(typeof(EncryptionConverter))]
        public List<ContactDto> Contacts { get; set; }
    }
}