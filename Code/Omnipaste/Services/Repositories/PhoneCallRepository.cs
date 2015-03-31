namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using Omnipaste.Entities;
    using Omnipaste.Models;

    public class PhoneCallRepository : ConversationRepository, IPhoneCallRepository
    {
        public PhoneCallRepository()
            : base("phoneCalls")
        {
        }

        #region IPhoneCallRepository Members

        public IObservable<RepositoryOperation<PhoneCallEntity>> GetOperationObservable()
        {
            return base.GetOperationObservable<PhoneCallEntity, LocalPhoneCallEntity, RemotePhoneCallEntity>();
        }

        public IObservable<RepositoryOperation<PhoneCallEntity>> Delete(string id)
        {
            return base.Delete<PhoneCallEntity, LocalPhoneCallEntity, RemotePhoneCallEntity>(id);
        }

        public IObservable<IEnumerable<PhoneCallEntity>> GetAll()
        {
            return base.GetAll<PhoneCallEntity, LocalPhoneCallEntity, RemotePhoneCallEntity>();
        }

        public IObservable<IEnumerable<PhoneCallEntity>> GetForContact(ContactEntity contactEntity)
        {
            return base.GetForContact<PhoneCallEntity, LocalPhoneCallEntity, RemotePhoneCallEntity>(contactEntity);
        }

        #endregion

        IObservable<IEnumerable<ConversationEntity>> IConversationRepository.GetConversationForContact(ContactEntity contactEntity)
        {
            return GetForContact(contactEntity);
        }

        public override IObservable<RepositoryOperation<T>> Save<T>(T item)
        {
            return base.Save<T, LocalPhoneCallEntity, RemotePhoneCallEntity>(item);
        }
    }
}
