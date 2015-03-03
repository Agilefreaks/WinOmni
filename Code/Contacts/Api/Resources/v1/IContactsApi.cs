namespace Contacts.Api.Resources.v1
{
    using System;
    using System.Collections.Generic;
    using global::Contacts.Models;
    using Refit;

    public interface IContactsApi
    {
        [Get("/user/contacts/{id}")]
        IObservable<ContactDto> Get(string id, [Header("Authorization")] string token);

        [Get("/user/contacts?from={from}")]
        IObservable<List<ContactDto>> GetUpdates(string from, [Header("Authorization")] string token);
    }
}