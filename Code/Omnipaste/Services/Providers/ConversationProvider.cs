namespace Omnipaste.Services.Providers
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
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

        public IObservable<Unit> SaveItem(IConversationItem item)
        {
            var call = item as Call;
            var result = Observable.Return(new Unit());
            if (call != null)
            {
                result = _callRepository.Save(call).Select(_ => new Unit());
            }
            else
            {
                var message = item as Message;
                if (message != null)
                {
                    result = _messageRepository.Save(message).Select(_ => new Unit());
                }
            }

            return result;
        }
    }
}