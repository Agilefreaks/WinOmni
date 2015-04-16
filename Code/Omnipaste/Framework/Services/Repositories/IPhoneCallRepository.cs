namespace Omnipaste.Framework.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using Omnipaste.Framework.Entities;

    public interface IPhoneCallRepository : IConversationRepository
    {
        IObservable<IEnumerable<PhoneCallEntity>> GetAll();

        IObservable<IEnumerable<PhoneCallEntity>> GetForContact(ContactEntity contactEntity);

        IObservable<RepositoryOperation<PhoneCallEntity>> GetOperationObservable();

        IObservable<RepositoryOperation<PhoneCallEntity>> Delete(string id);
    }
}