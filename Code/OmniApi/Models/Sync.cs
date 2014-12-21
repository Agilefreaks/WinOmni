namespace OmniApi.Models
{
    using System;

    public class Sync
    {
        public String What { get; set; }

        public string Identifier { get; set; }

        public Sync(string identifier)
        {
            Identifier = identifier;
            What = SyncWhatEnum.contacts.ToString();
        }
    }
}
