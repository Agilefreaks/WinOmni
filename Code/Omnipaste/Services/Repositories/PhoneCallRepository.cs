namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
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

        public IObservable<IEnumerable<PhoneCall>> GetForContact(ContactInfo contactInfo)
        {
            return base.GetForContact<PhoneCall, LocalPhoneCall, RemotePhoneCall>(contactInfo);
        }

        #endregion

        IObservable<IEnumerable<ConversationBaseModel>> IConversationRepository.GetForContact(ContactInfo contactInfo)
        {
            return GetForContact(contactInfo);
        }

        public override IObservable<RepositoryOperation<T>> Save<T>(T item)
        {
            return base.Save<T, LocalPhoneCall, RemotePhoneCall>(item);
        }
    }
}
