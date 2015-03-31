namespace Omnipaste.Services.Providers
{
    using System;
    using System.Reactive.Linq;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Presenters.Factories;
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

        public override IObservable<IConversationPresenter> GetItems()
        {
            return Observable.Empty<IConversationPresenter>();
        }

        protected override IObservable<IConversationPresenter> GetObservableForOperation(RepositoryMethodEnum method)
        {
            return
                SmsMessageRepository.GetOperationObservable()
                    .OnMethod(method)
                    .Where(o => o.Item is LocalSmsMessage)
                    .Select(o => SMSMessagePresenterFactory.Create(o.Item))
                    .Merge();
        }
    }
}