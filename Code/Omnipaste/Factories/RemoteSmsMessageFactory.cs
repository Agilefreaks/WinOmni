namespace Omnipaste.Factories
{
    using System;
    using System.Reactive.Linq;
    using Ninject;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using SMS.Models;

    public class RemoteSmsMessageFactory : IRemoteSmsMessageFactory
    {
        private readonly IMessageRepository _messageRepository;

        [Inject]
        public IContactRepository ContactRepository { get; set; }

        public RemoteSmsMessageFactory(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public IObservable<RemoteSmsMessage> Create(SmsMessageDto smsMessage)
        {
            return
                ContactRepository.GetOrCreateByPhoneNumber(smsMessage.PhoneNumber)
                    .Select(ci => new RemoteSmsMessage(smsMessage) { ContactInfo = ci })
                    .Select(m => _messageRepository.Save(m))
                    .Switch()
                    .Select(e => e.Item)
                    .Cast<RemoteSmsMessage>();
        }
    }
}