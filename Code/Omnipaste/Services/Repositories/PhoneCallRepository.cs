namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using Omnipaste.Models;

    public class PhoneCallRepository : ConversationRepository, IPhoneCallRepository
    {
        public PhoneCallRepository()
            : base("phoneCalls")
        {
        }

        #region IPhoneCallRepository Members

        public IObservable<RepositoryOperation<PhoneCall>> GetOperationObservable()
        {
            return base.GetOperationObservable<PhoneCall, LocalPhoneCall, RemotePhoneCall>();
        }

        public IObservable<RepositoryOperation<PhoneCall>> Delete(string id)
        {
            return base.Delete<PhoneCall, LocalPhoneCall, RemotePhoneCall>(id);
        }

        public IObservable<IEnumerable<PhoneCall>> GetAll()
        {
            return base.GetAll<PhoneCall, LocalPhoneCall, RemotePhoneCall>();
        }

        #endregion
    }
}
