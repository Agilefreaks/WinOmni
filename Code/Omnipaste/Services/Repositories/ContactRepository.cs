namespace Omnipaste.Services.Repositories
{
    using System;
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

        public IObservable<ContactInfo> GetByContactIdOrPhoneNumber(int? contactId, string phoneNumber)
        {
            return contactId.HasValue && contactId.Value != default(int) ?
                Get(c => c.ContactId == contactId).FirstAsync().Catch<ContactInfo, InvalidOperationException>(_ => GetByPhoneNumber(phoneNumber)) :
                GetByPhoneNumber(phoneNumber);
        }

        public IObservable<ContactInfo> CreateIfNone(IObservable<ContactInfo> observable, Func<ContactInfo, ContactInfo> builder)
        {
            return observable
                .Catch<ContactInfo, InvalidOperationException>(_ => 
                    Save(builder(new ContactInfo())).Select(ro => ro.Item));
        } 

        #endregion
    }
}