﻿namespace Contacts.Api.Resources.v1
{
    using System;
    using global::Contacts.Models;
    using Refit;

    public interface IContactsApi
    {
        [Get("/users/contacts")]
        IObservable<ContactList> Get(string identifier, [Header("Authorization")] string token);
    }
}