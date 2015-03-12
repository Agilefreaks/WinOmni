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
        private readonly IMessageRepository _messageRepository;

        [Inject]
        public IContactRepository ContactRepository { get; set; }

        public SmsMessageFactory(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public IObservable<T> Create<T>(SmsMessageDto smsMessageDto)
            where T : SmsMessage
        {
            SmsMessage smsMessage = (T)Activator.CreateInstance(typeof(T), smsMessageDto);
            return
                ContactRepository.GetOrCreateByPhoneNumber(smsMessageDto.PhoneNumber)
                    .Select(contact => ContactRepository.UpdateLastActivityTime(contact))
                    .Switch()
                    .Select(contact => _messageRepository.Save(smsMessage.SetContactInfo(contact)))
                    .Switch()
                    .Select(e => e.Item)
                    .Cast<T>();
        }
    }
}