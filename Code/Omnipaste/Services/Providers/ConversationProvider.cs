namespace Omnipaste.Services.Providers
{
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;

    public class ConversationProvider : IConversationProvider
    {
        private readonly IPhoneCallRepository _phoneCallRepository;

        private readonly IMessageRepository _messageRepository;

        public ConversationProvider(IMessageRepository messageRepository, IPhoneCallRepository phoneCallRepository)
        {
            _messageRepository = messageRepository;
            _phoneCallRepository = phoneCallRepository;
        }

        public IConversationContext ForContact(ContactInfo contactInfo)
        {
            return new ContactConversationContext(_messageRepository, _phoneCallRepository, contactInfo);
        }

        public IConversationContext All()
        {
            return new MergedConversationContext(_messageRepository, _phoneCallRepository);
        }
    }
}