namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using Omnipaste.Helpers;
    using Omnipaste.Models;

    public class ContactRepository : SecurePermanentRepository<ContactInfo>, IContactRepository
    {
        public ContactRepository()
            : base("contacts", null)
        {
        }

        #region IContactRepository Members

        public IObservable<ContactInfo> GetByPhoneNumber(string phoneNumber)
        {
            return Get(contact => contact.PhoneNumbers.Any(pn => PhoneNumberMatcher.IsMatch(pn.Number, phoneNumber)));
        }

        public IObservable<ContactInfo> GetOrCreateByPhoneNumber(string phoneNumber)
        {
            return
                GetByPhoneNumber(phoneNumber)
                    .Catch<ContactInfo, Exception>(
                        e =>
                            {
                                return
                                    Save(
                                        new ContactInfo
                                            {
                                                PhoneNumbers =
                                                    new[] { new PhoneNumber { Number = phoneNumber } }
                                            })
                                        .Select(o => o.Item);
                            });
        }

        public IObservable<ContactInfo> GetOrCreateByPhoneNumbers(IList<string> phoneNumbers)
        {
            return phoneNumbers.ToObservable().SelectMany(GetOrCreateByPhoneNumber);
        }

        public IObservable<ContactInfo> UpdateLastActivityTime(
            ContactInfo contactInfo,
            DateTime? lastActivityTime = null)
        {
            contactInfo.LastActivityTime = DateTime.Now;
            return Save(contactInfo).Select(o => o.Item);
        }

        public IObservable<ContactInfo> GetByContactIdOrPhoneNumber(int? contactId, string phoneNumber)
        {
            return contactId.HasValue ?
                Get(c => c.ContactId == contactId).FirstAsync().Catch<ContactInfo, InvalidOperationException>(_ => GetByPhoneNumber(phoneNumber)) :
                GetByPhoneNumber(phoneNumber);
        }

        public IObservable<ContactInfo> CreateIfNone(IObservable<ContactInfo> observable, Func<ContactInfo, ContactInfo> builder)
        {
            return observable.Catch<ContactInfo, InvalidOperationException>(_ => Save(builder(new ContactInfo())).Select(ro => ro.Item));
        } 

        #endregion
    }
}