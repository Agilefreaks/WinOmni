namespace Omnipaste.Services.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;

    public class ContactConversationContext : ConversationContext
    {
        private readonly ContactInfo _contactInfo;

        public ContactConversationContext(
            IMessageRepository messageRepository,
            ICallRepository callRepository,
            ContactInfo contactInfo)
            : base(messageRepository, callRepository)
        {
            _contactInfo = contactInfo;
        }

        public override IObservable<IEnumerable<IConversationItem>> GetItems()
        {
            return
                Observable.When(
                    MessageRepository.GetByContact(_contactInfo)
                        .And(CallRepository.GetByContact(_contactInfo))
                        .Then((messages, calls) => messages.Cast<IConversationItem>().Concat(calls)));
        }

        protected override IObservable<IConversationItem> GetObservableForContactAndOperation(
            RepositoryMethodEnum method)
        {
            return
                MessageRepository.OperationObservable.OnMethod(method)
                    .ForContact(_contactInfo)
                    .Select(o => o.Item)
                    .Merge(
                        CallRepository.OperationObservable.OnMethod(method)
                            .ForContact(_contactInfo)
                            .Select(o => o.Item)
                            .Cast<IConversationItem>());
        }
    }
}