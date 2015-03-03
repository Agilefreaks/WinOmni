namespace Omnipaste.Services.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;

    public class MergedConversationContext : ConversationContext
    {
        public MergedConversationContext(IMessageRepository messageRepository, IPhoneCallRepository phoneCallRepository)
            : base(messageRepository, phoneCallRepository)
        {
        }

        public override IObservable<IEnumerable<IConversationItem>> GetItems()
        {
            return
                Observable.When(
                    MessageRepository.GetAll()
                        .And(PhoneCallRepository.GetAll())
                        .Then((messages, calls) => messages.Cast<IConversationItem>().Concat(calls)));
        }

        protected override IObservable<IConversationItem> GetObservableForContactAndOperation(
            RepositoryMethodEnum method)
        {
            return
                MessageRepository.OperationObservable.OnMethod(method)
                    .Select(o => o.Item)
                    .Merge(
                        PhoneCallRepository.OperationObservable.OnMethod(method)
                            .Select(o => o.Item)
                            .Cast<IConversationItem>());
        }
    }
}