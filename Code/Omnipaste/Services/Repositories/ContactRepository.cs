namespace Omnipaste.Services.Repositories
{
    using System;
    using Omnipaste.Helpers;
    using Omnipaste.Models;

    public class ContactRepository : InMemoryRepository<ContactInfo>, IContactRepository
    {
        public IObservable<ContactInfo> GetByPhoneNumber(string phoneNumber)
        {
            return Get(contact => PhoneNumberMatcher.IsMatch(contact.Phone, phoneNumber));
        }
    }
}
