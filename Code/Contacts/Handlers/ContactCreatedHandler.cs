namespace Contacts.Handlers
{
    using System;
    using Contacts.Api.Resources.v1;
    using Contacts.Models;
    using OmniCommon.Handlers;
    using OmniCommon.Models;

    public class ContactCreatedHandler : ResourceHandler<ContactDto>, IContactCreatedHandler
    {
        private readonly IContacts _contacts;

        public ContactCreatedHandler(IContacts contacts)
        {
            _contacts = contacts;
        }

        public override string HandledMessageType
        {
            get
            {
                return "contact_created;contact_updated";
            }
        }

        protected override IObservable<ContactDto> CreateResult(OmniMessage value)
        {
            var contactId = value.GetPayload("id");
            
            return _contacts.Get(contactId);
        }
    }
}