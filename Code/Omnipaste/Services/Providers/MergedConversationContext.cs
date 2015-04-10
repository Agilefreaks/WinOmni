namespace Omnipaste.Services.Providers
{
    using System;
    using System.Reactive.Linq;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;
    using Omnipaste.Framework.Models.Factories;
    using Omnipaste.Services.Repositories;

    public class MergedConversationContext : ConversationContext
    {
        public MergedConversationContext(
            ISmsMessageRepository smsMessageRepository,
            IPhoneCallRepository phoneCallRepository,
            IPhoneCallModelFactory phoneCallModelFactory,
            ISmsMessageModelFactory smsMessageModelFactory)
            : base(smsMessageRepository, phoneCallRepository, phoneCallModelFactory, smsMessageModelFactory)
        {
        }

        public override IObservable<IConversationModel> GetItems()
        {
            return Observable.Empty<IConversationModel>();
        }

        protected override IObservable<IConversationModel> GetObservableForOperation(RepositoryMethodEnum method)
        {
            return
                SmsMessageRepository.GetOperationObservable()
                    .OnMethod(method)
                    .Where(o => o.Item is LocalSmsMessageEntity)
                    .Select(o => SmsMessageModelFactory.Create(o.Item))
                    .Merge();
        }
    }
}
