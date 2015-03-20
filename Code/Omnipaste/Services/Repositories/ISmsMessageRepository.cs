namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using Omnipaste.Models;

    public interface ISmsMessageRepository : IConversationRepository
    {
        IObservable<IEnumerable<SmsMessage>> GetAll();

        IObservable<RepositoryOperation<SmsMessage>> GetOperationObservable();

        IObservable<RepositoryOperation<SmsMessage>> Delete(string id);

    }
}