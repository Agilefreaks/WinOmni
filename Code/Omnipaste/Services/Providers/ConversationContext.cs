namespace Omnipaste.Services.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Reactive;
    using System.Reactive.Linq;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;

    public abstract class ConversationContext : IConversationContext
    {
        private IObservable<IConversationItem> _itemRemoved;

        private IObservable<IConversationItem> _itemUpdated;

        private IObservable<IConversationItem> _updated;

        protected readonly IPhoneCallRepository PhoneCallRepository;

        protected readonly IMessageRepository MessageRepository;

        protected ConversationContext(IMessageRepository messageRepository, IPhoneCallRepository phoneCallRepository)
        {
            MessageRepository = messageRepository;
            PhoneCallRepository = phoneCallRepository;
        }

        public IObservable<IConversationItem> ItemChanged
        {
            get
            {
                return _itemUpdated ?? (_itemUpdated = GetObservableForContactAndOperation(RepositoryMethodEnum.Changed));
            }
        }

        public IObservable<IConversationItem> ItemRemoved
        {
            get
            {
                return _itemRemoved ?? (_itemRemoved = GetObservableForContactAndOperation(RepositoryMethodEnum.Delete));
            }
        }

        public IObservable<IConversationItem> Updated
        {
            get
            {
                return _updated ?? (_updated = ItemChanged.Merge(ItemRemoved));
            }
        }

        public abstract IObservable<IEnumerable<IConversationItem>> GetItems();

        public virtual IObservable<Unit> SaveItem(IConversationItem item)
        {
            var call = item as PhoneCall;
            var result = Observable.Return(new Unit());
            if (call != null)
            {
                result = PhoneCallRepository.Save(call).Select(_ => new Unit());
            }
            else
            {
                var message = item as SmsMessage;
                if (message != null)
                {
                    result = MessageRepository.Save(message).Select(_ => new Unit());
                }
            }

            return result;
        }

        protected abstract IObservable<IConversationItem> GetObservableForContactAndOperation(
            RepositoryMethodEnum method);
    }
}