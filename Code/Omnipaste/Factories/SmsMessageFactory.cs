namespace Omnipaste.Factories
{
    using System;
    using System.Reactive.Linq;
    using Ninject;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using SMS.Models;

    public class SmsMessageFactory : ISmsMessageFactory
    {
        private readonly ISmsMessageRepository _smsMessageRepository;

        [Inject]
        public IContactRepository ContactRepository { get; set; }

        public SmsMessageFactory(ISmsMessageRepository smsMessageRepository)
        {
            _smsMessageRepository = smsMessageRepository;
        }

        public IObservable<T> Create<T>(SmsMessageDto smsMessageDto)
            where T : SmsMessage
        {
            return
                ContactRepository.GetOrCreateByPhoneNumbers(smsMessageDto.PhoneNumberList)
                    .Select(contact => ContactRepository.UpdateLastActivityTime(contact))
                    .Switch()
                    .Select(contact =>
                        {
                            var smsMessage = (T)Activator.CreateInstance(typeof(T), smsMessageDto);
                            smsMessage.SetContactInfoUniqueId(contact.UniqueId);
                            return _smsMessageRepository.Save(smsMessage);
                        })
                    .Switch()
                    .Select(e => e.Item)
                    .Cast<T>();
        }
    }
}