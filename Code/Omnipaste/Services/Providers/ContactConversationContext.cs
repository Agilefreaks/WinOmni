namespace Omnipaste.Services.Providers
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Presenters.Factories;
    using Omnipaste.Services.Repositories;

    public class ContactConversationContext : ConversationContext
    {
        private readonly ContactInfo _contactInfo;

        public ContactConversationContext(
            ISmsMessageRepository smsMessageRepository,
            IPhoneCallRepository phoneCallRepository,
            IPhoneCallPresenterFactory phoneCallPresenterFactory,
            ISmsMessagePresenterFactory smsMessagePresenterFactory,
            ContactInfo contactInfo)
            : base(smsMessageRepository, phoneCallRepository, phoneCallPresenterFactory, smsMessagePresenterFactory)
        {
            _contactInfo = contactInfo;
        }

        public override IObservable<IConversationPresenter> GetItems()
        {
            return
                SmsMessageRepository.GetForContact(_contactInfo)
                    .SelectMany(messages => messages.Select(m => SMSMessagePresenterFactory.Create(m)))
                    .Merge(
                        PhoneCallRepository.GetForContact(_contactInfo)
                            .SelectMany(calls => calls.Select(c => PhoneCallPresenterFactory.Create(c))))
                    .Switch();
        }

        protected override IObservable<IConversationPresenter> GetObservableForOperation(RepositoryMethodEnum method)
        {
            return
                SmsMessageRepository.GetOperationObservable()
                    .OnMethod(method)
                    .ForContact(_contactInfo)
                    .Select(o => SMSMessagePresenterFactory.Create(o.Item))
                    .Merge(
                        PhoneCallRepository.GetOperationObservable()
                            .OnMethod(method)
                            .ForContact(_contactInfo)
                            .Select(o => PhoneCallPresenterFactory.Create(o.Item)))
                    .Cast<IConversationPresenter>();
        }
    }
}