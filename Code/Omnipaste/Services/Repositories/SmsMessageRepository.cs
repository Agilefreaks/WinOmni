namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using Omnipaste.Models;

    public class SmsMessageRepository : ConversationRepository, ISmsMessageRepository
    {
        public SmsMessageRepository()
            : base("smsMessages")
        {
        }

        #region IPhoneCallRepository Members

        public IObservable<RepositoryOperation<SmsMessage>> GetOperationObservable()
        {
            return base.GetOperationObservable<SmsMessage, LocalSmsMessage, RemoteSmsMessage>();
        }

        public IObservable<RepositoryOperation<SmsMessage>> Delete(string id)
        {
            return base.Delete<SmsMessage, LocalSmsMessage, RemoteSmsMessage>(id);
        }

        public IObservable<IEnumerable<SmsMessage>> GetAll()
        {
            return base.GetAll<SmsMessage, LocalSmsMessage, RemoteSmsMessage>();
        }

        public IObservable<IEnumerable<SmsMessage>> GetForContact(ContactInfo contactInfo)
        {
            return base.GetForContact<SmsMessage, LocalSmsMessage, RemoteSmsMessage>(contactInfo);
        }

        #endregion

        IObservable<IEnumerable<ConversationBaseModel>> IConversationRepository.GetForContact(ContactInfo contactInfo)
        {
            return GetForContact(contactInfo);
        }

        public override IObservable<RepositoryOperation<T>> Save<T>(T item)
        {
            return base.Save<T, LocalSmsMessage, RemoteSmsMessage>(item);
        }
    }
}
