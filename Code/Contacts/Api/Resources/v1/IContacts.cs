namespace Contacts.Api.Resources.v1
{
    using System;
    using System.Collections.Generic;
    using global::Contacts.Dto;
    using global::Contacts.Models;

    public interface IContacts
    {
        IObservable<ContactDto> Get(string id);

        IObservable<List<ContactDto>> GetUpdates(DateTime from);
    }
}