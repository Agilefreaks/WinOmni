namespace Omnipaste.Factories
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using Contacts.Dto;
    using Omnipaste.Entities;
    using Omnipaste.Services.Repositories;

    public class ContactFactory : IContactFactory
    {
        private readonly IContactRepository _contactRepository;

        public ContactFactory(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        #region IContactFactory Members

        public IObservable<ContactEntity> Create(ContactDto contactDto)
        {
            return Create(contactDto, null);
        }

        public IObservable<ContactEntity> Create(ContactDto contactDto, DateTime? lastActivityTime)
        {
            var contactEntity = new ContactEntity(contactDto);

            return
                _contactRepository.CreateIfNone(
                    _contactRepository.GetByContactIdOrPhoneNumber(contactDto.ContactId, contactDto.PhoneNumbers.First().Number),
                    c => c.AddPhoneNumber(contactEntity.PhoneNumber).SetContactId(contactEntity.ContactId)
                )
                .Select(
                    ci =>
                        {
                            var info = lastActivityTime != null ? ci : contactEntity.SetUniqueId(ci.UniqueId);
                            return _contactRepository.Save(info.SetLastActivityTime(lastActivityTime ?? ci.LastActivityTime));
                        })
                        .Switch()
                        .Select(ro => ro.Item);
        }

        #endregion
    }
}