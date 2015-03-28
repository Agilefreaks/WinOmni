﻿namespace Omnipaste.Factories
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

        #region IContactFactory Members

        public IObservable<ContactInfo> Create(ContactDto contactDto)
        {
            return Create(contactDto, null);
        }

        public IObservable<ContactInfo> Create(ContactDto contactDto, DateTime? lastActivityTime)
        {
            var contactInfo = new ContactInfo(contactDto);

            return
                _contactRepository.CreateIfNone(
                    _contactRepository.GetByContactIdOrPhoneNumber(
                        contactDto.ContactId,
                        contactDto.PhoneNumbers.First().Number),
                    c => c.AddPhoneNumber(contactInfo.PhoneNumber).SetContactId(contactInfo.ContactId))
                    .Select(
                        ci =>
                        _contactRepository.Save(
                            contactInfo.SetUniqueId(ci.UniqueId).SetLastActivityTime(lastActivityTime ?? ci.LastActivityTime)))
                    .Switch()
                    .Select(ro => ro.Item);
        }

        #endregion
    }
}