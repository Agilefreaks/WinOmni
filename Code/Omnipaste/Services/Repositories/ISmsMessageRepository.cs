namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using Omnipaste.Entities;
    using Omnipaste.Models;

    public interface ISmsMessageRepository : IConversationRepository
    {
        IObservable<IEnumerable<SmsMessageEntity>> GetAll();

        IObservable<IEnumerable<SmsMessageEntity>> GetForContact(ContactEntity contactEntity);

        IObservable<RepositoryOperation<SmsMessageEntity>> GetOperationObservable();

        IObservable<RepositoryOperation<SmsMessageEntity>> Delete(string id);

    }
}