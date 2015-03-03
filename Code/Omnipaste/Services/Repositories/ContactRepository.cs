namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using Omnipaste.Helpers;
    using Omnipaste.Models;

    public class ContactRepository : InMemoryRepository<ContactInfo>, IContactRepository
    {
        public IObservable<ContactInfo> GetByPhoneNumber(string phoneNumber)
        {
            return Get(contact => contact.PhoneNumbers.Any(pn => PhoneNumberMatcher.IsMatch(pn.Number, phoneNumber))); 
        }

        public IObservable<ContactInfo> GetOrCreateByPhoneNumber(string phoneNumber)
        {
            return GetByPhoneNumber(phoneNumber).Select(c => 
                c != null 
                    ? Observable.Repeat(c, 1) 
                    : Save(new ContactInfo { PhoneNumbers = new[] { new PhoneNumber { Number = phoneNumber } } }).Select(o => o.Item)
                ).Switch();
        }
    }
}
