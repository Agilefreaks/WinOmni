namespace Omnipaste.Framework.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using Omnipaste.Framework.Entities;

    public interface ISmsMessageRepository : IConversationRepository
    {
        IObservable<IEnumerable<SmsMessageEntity>> GetAll();

        IObservable<IEnumerable<SmsMessageEntity>> GetForContact(ContactEntity contactEntity);

        IObservable<RepositoryOperation<SmsMessageEntity>> GetOperationObservable();

        IObservable<RepositoryOperation<SmsMessageEntity>> Delete(string id);

    }
}