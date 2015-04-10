namespace Omnipaste.Services.Providers
{
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Models.Factories;
    using Omnipaste.Services.Repositories;

    public class ConversationProvider : IConversationProvider
    {
        private readonly IPhoneCallRepository _phoneCallRepository;

        private readonly IPhoneCallModelFactory _phoneCallModelFactory;

        private readonly ISmsMessageModelFactory _smsMessageModelFactory;

        private readonly ISmsMessageRepository _smsMessageRepository;

        public ConversationProvider(
            ISmsMessageRepository smsMessageRepository,
            IPhoneCallRepository phoneCallRepository,
            IPhoneCallModelFactory phoneCallModelFactory,
            ISmsMessageModelFactory smsMessageModelFactory)
        {
            _smsMessageRepository = smsMessageRepository;
            _phoneCallRepository = phoneCallRepository;
            _phoneCallModelFactory = phoneCallModelFactory;
            _smsMessageModelFactory = smsMessageModelFactory;
        }

        #region IConversationProvider Members

        public IConversationContext ForContact(ContactEntity contactEntity)
        {
            return new ContactConversationContext(_smsMessageRepository, _phoneCallRepository, _phoneCallModelFactory, _smsMessageModelFactory, contactEntity);
        }

        public IConversationContext All()
        {
            return new MergedConversationContext(_smsMessageRepository, _phoneCallRepository, _phoneCallModelFactory, _smsMessageModelFactory);
        }

        #endregion
    }
}
