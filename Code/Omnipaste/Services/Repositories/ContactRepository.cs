namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using Omnipaste.Entities;
    using Omnipaste.Helpers;

    public class ContactRepository : SecurePermanentRepository<ContactEntity>, IContactRepository
    {
        public ContactRepository()
            : base("contacts", null)
        {
        }

        #region IContactRepository Members

        public IObservable<ContactEntity> GetByPhoneNumber(string phoneNumber)
        {
            return Get(contact => contact.PhoneNumbers.Any(pn => PhoneNumberMatcher.IsMatch(pn.Number, phoneNumber)));
        }

        public IObservable<ContactEntity> GetByContactIdOrPhoneNumber(int? contactId, string phoneNumber)
        {
            return contactId.HasValue && contactId.Value != default(int) ?
                Get(c => c.ContactId == contactId).FirstAsync().Catch<ContactEntity, InvalidOperationException>(_ => GetByPhoneNumber(phoneNumber)) :
                GetByPhoneNumber(phoneNumber);
        }

        public IObservable<ContactEntity> CreateIfNone(IObservable<ContactEntity> observable, Func<ContactEntity, ContactEntity> builder)
        {
            return observable
                .Catch<ContactEntity, InvalidOperationException>(_ => 
                    Save(builder(new ContactEntity())).Select(ro => ro.Item));
        } 

        #endregion
    }
}