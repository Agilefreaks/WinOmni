namespace Contacts.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using OmniApi.Support.Converters;

    public class ContactList
    {
        [JsonConverter(typeof(EncryptionConverter))]
        public List<Contact> Contacts { get; set; }
    }
}
