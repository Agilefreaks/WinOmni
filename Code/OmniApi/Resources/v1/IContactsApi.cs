﻿namespace OmniApi.Resources.v1
{
    using System;
    using OmniApi.Models;
    using Refit;

    public interface IContactsApi
    {
        [Get("/contacts")]
        IObservable<ContactList> Get([Header("Authorization")] string token);

        [Post("/contacts/sync")]
        IObservable<EmptyModel> Sync([Header("Authorization")] string token);
    }
}