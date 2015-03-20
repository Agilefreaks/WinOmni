namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using Omnipaste.Models;

    public interface IPhoneCallRepository : IConversationRepository
    {
        IObservable<IEnumerable<PhoneCall>> GetAll();

        IObservable<RepositoryOperation<PhoneCall>> GetOperationObservable();

        IObservable<RepositoryOperation<PhoneCall>> Delete(string id);
    }
}