namespace Omnipaste.Services.Providers
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Presenters.Factories;
    using Omnipaste.Services.Repositories;

    public class ContactConversationContext : ConversationContext
    {
        private readonly ContactEntity _contactEntity;

        public ContactConversationContext(
            ISmsMessageRepository smsMessageRepository,
            IPhoneCallRepository phoneCallRepository,
            IPhoneCallPresenterFactory phoneCallPresenterFactory,
            ISmsMessagePresenterFactory smsMessagePresenterFactory,
            ContactEntity contactEntity)
            : base(smsMessageRepository, phoneCallRepository, phoneCallPresenterFactory, smsMessagePresenterFactory)
        {
            _contactEntity = contactEntity;
        }

        public override IObservable<IConversationPresenter> GetItems()
        {
            return
                SmsMessageRepository.GetForContact(_contactEntity)
                    .SelectMany(messages => messages.Select(m => SMSMessagePresenterFactory.Create(m))).Merge()
                    .Merge(
                        PhoneCallRepository.GetForContact(_contactEntity)
                            .SelectMany(calls => calls.Select(c => PhoneCallPresenterFactory.Create(c))).Merge());
        }

        protected override IObservable<IConversationPresenter> GetObservableForOperation(RepositoryMethodEnum method)
        {
            return
                SmsMessageRepository.GetOperationObservable()
                    .OnMethod(method)
                    .ForContact(_contactEntity)
                    .Select(o => SMSMessagePresenterFactory.Create(o.Item)).Merge()
                    .Merge(
                        PhoneCallRepository.GetOperationObservable()
                            .OnMethod(method)
                            .ForContact(_contactEntity)
                            .Select(o => PhoneCallPresenterFactory.Create(o.Item)).Merge());
        }
    }
}