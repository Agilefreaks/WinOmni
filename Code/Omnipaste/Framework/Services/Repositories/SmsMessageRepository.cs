namespace Omnipaste.Framework.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using Omnipaste.Framework.Entities;

    public class SmsMessageRepository : ConversationRepository, ISmsMessageRepository
    {
        public SmsMessageRepository()
            : base("smsMessages")
        {
        }

        #region IPhoneCallRepository Members

        public IObservable<RepositoryOperation<SmsMessageEntity>> GetOperationObservable()
        {
            return base.GetOperationObservable<SmsMessageEntity, LocalSmsMessageEntity, RemoteSmsMessageEntity>();
        }

        public IObservable<RepositoryOperation<SmsMessageEntity>> Delete(string id)
        {
            return base.Delete<SmsMessageEntity, LocalSmsMessageEntity, RemoteSmsMessageEntity>(id);
        }

        public IObservable<IEnumerable<SmsMessageEntity>> GetAll()
        {
            return base.GetAll<SmsMessageEntity, LocalSmsMessageEntity, RemoteSmsMessageEntity>();
        }

        public IObservable<IEnumerable<SmsMessageEntity>> GetForContact(ContactEntity contactEntity)
        {
            return base.GetForContact<SmsMessageEntity, LocalSmsMessageEntity, RemoteSmsMessageEntity>(contactEntity);
        }

        #endregion

        IObservable<IEnumerable<ConversationEntity>> IConversationRepository.GetConversationForContact(ContactEntity contactEntity)
        {
            return GetForContact(contactEntity);
        }

        public override IObservable<RepositoryOperation<T>> Save<T>(T item)
        {
            return base.Save<T, LocalSmsMessageEntity, RemoteSmsMessageEntity>(item);
        }
    }
}
