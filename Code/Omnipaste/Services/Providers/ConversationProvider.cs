namespace Omnipaste.Services.Providers
{
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;

    public class ConversationProvider : IConversationProvider
    {
        private readonly ICallRepository _callRepository;

        private readonly IMessageRepository _messageRepository;

        public ConversationProvider(IMessageRepository messageRepository, ICallRepository callRepository)
        {
            _messageRepository = messageRepository;
            _callRepository = callRepository;
        }

        public IConversationContext ForContact(ContactInfo contactInfo)
        {
            return new ContactConversationContext(_messageRepository, _callRepository, contactInfo);
        }

        public IConversationContext All()
        {
            return new MergedConversationContext(_messageRepository, _callRepository);
        }
    }
}