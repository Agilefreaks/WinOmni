﻿namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using Omnipaste.Entities;
    using Omnipaste.Models;

    public interface IPhoneCallRepository : IConversationRepository
    {
        IObservable<IEnumerable<PhoneCallEntity>> GetAll();

        IObservable<IEnumerable<PhoneCallEntity>> GetForContact(ContactEntity contactEntity);

        IObservable<RepositoryOperation<PhoneCallEntity>> GetOperationObservable();

        IObservable<RepositoryOperation<PhoneCallEntity>> Delete(string id);
    }
}