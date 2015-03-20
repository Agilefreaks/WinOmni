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

        #endregion
    }
}
