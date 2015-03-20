namespace Omnipaste.Services.Providers
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
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
            return
                SmsMessageRepository.GetAll()
                    .SelectMany(messages => messages.Select(m => SMSMessagePresenterFactory.Create(m)))
                    .Merge(
                        PhoneCallRepository.GetAll()
                            .SelectMany(calls => calls.Select(c => PhoneCallPresenterFactory.Create(c))))
                    .Switch();
        }

        protected override IObservable<IConversationPresenter> GetObservableForOperation(RepositoryMethodEnum method)
        {
            return
                SmsMessageRepository.GetOperationObservable()
                    .OnMethod(method)
                    .SelectMany(o => SMSMessagePresenterFactory.Create(o.Item))
                    .Merge(
                        PhoneCallRepository.GetOperationObservable()
                            .OnMethod(method)
                            .SelectMany(o => PhoneCallPresenterFactory.Create(o.Item))
                            .Cast<IConversationPresenter>());
        }
    }
}