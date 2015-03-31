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
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using SMS.Dto;

    public class SmsMessageFactory : ISmsMessageFactory
    {
        private readonly ISmsMessageRepository _smsMessageRepository;

        public SmsMessageFactory(ISmsMessageRepository smsMessageRepository)
        {
            _smsMessageRepository = smsMessageRepository;
        }

        [Inject]
        public IContactFactory ContactFactory { get; set; }

        #region ISmsMessageFactory Members

        public IObservable<T> Create<T>(SmsMessageDto smsMessageDto) where T : SmsMessageEntity
        {
            var contactDto = new ContactDto { PhoneNumbers = new List<PhoneNumberDto> { new PhoneNumberDto(smsMessageDto.PhoneNumber) }, ContactId =  smsMessageDto.ContactId ?? default(int) };

            return ContactFactory.Create(contactDto, TimeHelper.UtcNow).Select(
                contact =>
                    {
                        var smsMessage = (T)Activator.CreateInstance(typeof(T), smsMessageDto);
                        smsMessage.SetContactInfoUniqueId(contact.UniqueId);
                        return _smsMessageRepository.Save(smsMessage);
                    }).Switch().Select(e => e.Item).Cast<T>();
        }

        #endregion
    }
}