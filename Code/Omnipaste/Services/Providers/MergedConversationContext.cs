namespace Omnipaste.Services.Providers
{
    using System;
    using System.Reactive.Linq;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Models.Factories;
    using Omnipaste.Services.Repositories;

    public class MergedConversationContext : ConversationContext
    {
        public MergedConversationContext(
            ISmsMessageRepository smsMessageRepository,
            IPhoneCallRepository phoneCallRepository,
            IPhoneCallPresenterFactory phoneCallPresenterFactory,
            ISmsMessagePresenterFactory smsMessagePresenterFactory)
            : base(smsMessageRepository, phoneCallRepository, phoneCallPresenterFactory, smsMessagePresenterFactory)
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
                    .Select(o => SMSMessagePresenterFactory.Create(o.Item))
                    .Merge();
        }
    }
}
