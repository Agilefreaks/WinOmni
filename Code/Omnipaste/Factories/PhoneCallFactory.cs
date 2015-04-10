namespace Omnipaste.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using Contacts.Dto;
    using Contacts.Models;
    using Ninject;
    using OmniCommon.Helpers;
    using Omnipaste.Entities;
    using Omnipaste.Services.Repositories;
    using PhoneCalls.Dto;

    public class PhoneCallFactory : IPhoneCallFactory
    {
        private readonly IPhoneCallRepository _phoneCallRepository;

        [Inject]
        public IContactFactory ContactFactory { get; set; }

        public PhoneCallFactory(IPhoneCallRepository phoneCallRepository)
        {
            _phoneCallRepository = phoneCallRepository;
        }

        public IObservable<T> Create<T>(PhoneCallDto phoneCallDto)
            where T : PhoneCallEntity
        {
            var contactDto = new ContactDto { PhoneNumbers = new List<PhoneNumberDto> { new PhoneNumberDto(phoneCallDto.Number) }, ContactId = phoneCallDto.ContactId ?? default(int) };

            return
                ContactFactory.Create(contactDto, TimeHelper.UtcNow)
                    .Select(
                        contact =>
                            {
                                var phoneCall = (T)Activator.CreateInstance(typeof(T), phoneCallDto);
                                return _phoneCallRepository.Save((T)phoneCall.SetContactUniqueId(contact.UniqueId));
                            })
                    .Switch()
                    .Select(e => e.Item)
                    .Cast<T>();

        }
    }
}