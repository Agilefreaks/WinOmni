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

        public SmsMessageFactory(ISmsMessageRepository smsMessageRepository)
        {
            _smsMessageRepository = smsMessageRepository;
        }

        [Inject]
        public IContactRepository ContactRepository { get; set; }

        #region ISmsMessageFactory Members

        public IObservable<T> Create<T>(SmsMessageDto smsMessageDto) where T : SmsMessage
        {
            var createIfNone =
                ContactRepository.CreateIfNone(
                    ContactRepository.GetByContactIdOrPhoneNumber(smsMessageDto.ContactId, smsMessageDto.PhoneNumber),
                    c => c.AddPhoneNumber(smsMessageDto.PhoneNumber).SetContactId(smsMessageDto.ContactId));

            return
                createIfNone.Select(contact => ContactRepository.UpdateLastActivityTime(contact))
                    .Switch()
                    .Select(
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