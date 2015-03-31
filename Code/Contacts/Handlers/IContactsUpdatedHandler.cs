namespace Contacts.Handlers
{
    using System.Collections.Generic;
    using Contacts.Dto;
    using Contacts.Models;
    using OmniCommon.Handlers;

    public interface IContactsUpdatedHandler : IResourceHandler<List<ContactDto>>
    {
    }
}