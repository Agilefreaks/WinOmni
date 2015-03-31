namespace Omnipaste.Services.Providers
{
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Presenters.Factories;
    using Omnipaste.Services.Repositories;

    public class ConversationProvider : IConversationProvider
    {
        private readonly IPhoneCallRepository _phoneCallRepository;

        private readonly IPhoneCallPresenterFactory _phoneCallPresenterFactory;

        private readonly ISmsMessagePresenterFactory _smsMessagePresenterFactory;

        private readonly ISmsMessageRepository _smsMessageRepository;

        public ConversationProvider(
            ISmsMessageRepository smsMessageRepository,
            IPhoneCallRepository phoneCallRepository,
            IPhoneCallPresenterFactory phoneCallPresenterFactory,
            ISmsMessagePresenterFactory smsMessagePresenterFactory)
        {
            _smsMessageRepository = smsMessageRepository;
            _phoneCallRepository = phoneCallRepository;
            _phoneCallPresenterFactory = phoneCallPresenterFactory;
            _smsMessagePresenterFactory = smsMessagePresenterFactory;
        }

        #region IConversationProvider Members

        public IConversationContext ForContact(ContactEntity contactEntity)
        {
            return new ContactConversationContext(_smsMessageRepository, _phoneCallRepository, _phoneCallPresenterFactory, _smsMessagePresenterFactory, contactEntity);
        }

        public IConversationContext All()
        {
            return new MergedConversationContext(_smsMessageRepository, _phoneCallRepository, _phoneCallPresenterFactory, _smsMessagePresenterFactory);
        }

        #endregion
    }
}
