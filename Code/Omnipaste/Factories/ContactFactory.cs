namespace Omnipaste.Factories
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using Contacts.Models;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;

    public class ContactFactory : IContactFactory
    {
        private readonly IContactRepository _contactRepository;

        public ContactFactory(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public IObservable<ContactInfo> Create(ContactDto contactDto)
        {
            var contactInfo = new ContactInfo(contactDto);

            return _contactRepository.GetByPhoneNumber(contactDto.PhoneNumbers.First().Number)
                .Catch<ContactInfo, Exception>(_ => Observable.Return(contactInfo))
                .Select(ci => _contactRepository.Save(contactInfo.SetUniqueId(ci.UniqueId).SetLastActivityTime(ci.LastActivityTime)))
                .Switch()
                .Select(ro => ro.Item);
        }
    }
}