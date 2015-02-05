namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.Models;

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
            return new ConversationContext(this, contactInfo);
        }

        protected IObservable<IConversationItem> GetObservableForContactAndOperation(
            ContactInfo contactInfo,
            RepositoryMethodEnum method)
        {
            return
                _messageRepository.OperationObservable.OnMethod(method)
                    .ForContact(contactInfo)
                    .Select(o => o.Item)
                    .Merge(
                        _callRepository.OperationObservable.OnMethod(method)
                            .ForContact(contactInfo)
                            .Select(o => o.Item)
                            .Cast<IConversationItem>());
        }

        protected IObservable<IEnumerable<IConversationItem>> GetAllCurrentItems(ContactInfo contactInfo)
        {
            return
                Observable.When(
                    _messageRepository.GetByContact(contactInfo)
                        .And(_callRepository.GetByContact(contactInfo))
                        .Then((messages, calls) => messages.Cast<IConversationItem>().Concat(calls)));
        }

        protected IObservable<Unit> SaveItem(IConversationItem item)
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

        protected class ConversationContext : IConversationContext
        {
            private readonly ContactInfo _contactInfo;

            private readonly ConversationProvider _conversationProvider;

            public ConversationContext(ConversationProvider conversationProvider, ContactInfo contactInfo)
            {
                _conversationProvider = conversationProvider;
                _contactInfo = contactInfo;
                ItemRemoved = conversationProvider.GetObservableForContactAndOperation(
                    contactInfo,
                    RepositoryMethodEnum.Delete);
                ItemAdded = conversationProvider.GetObservableForContactAndOperation(
                    contactInfo,
                    RepositoryMethodEnum.Create);
                ItemUpdated = conversationProvider.GetObservableForContactAndOperation(
                    contactInfo,
                    RepositoryMethodEnum.Update);
                Updated = ItemAdded.Merge(ItemUpdated).Merge(ItemRemoved);
            }

            public IObservable<IConversationItem> ItemUpdated { get; private set; }

            public IObservable<IConversationItem> ItemAdded { get; private set; }

            public IObservable<IConversationItem> ItemRemoved { get; private set; }

            public IObservable<IConversationItem> Updated { get; private set; }

            public IObservable<IEnumerable<IConversationItem>> GetItems()
            {
                return _conversationProvider.GetAllCurrentItems(_contactInfo);
            }

            public IObservable<Unit> Save(IConversationItem item)
            {
                return _conversationProvider.SaveItem(item);
            }
        }
    }
}