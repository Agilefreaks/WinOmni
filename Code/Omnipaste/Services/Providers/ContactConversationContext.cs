namespace Omnipaste.Services.Providers
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;
    using Omnipaste.Framework.Models.Factories;
    using Omnipaste.Services.Repositories;

    public class ContactConversationContext : ConversationContext
    {
        private readonly ContactEntity _contactEntity;

        public ContactConversationContext(
            ISmsMessageRepository smsMessageRepository,
            IPhoneCallRepository phoneCallRepository,
            IPhoneCallModelFactory phoneCallModelFactory,
            ISmsMessageModelFactory smsMessageModelFactory,
            ContactEntity contactEntity)
            : base(smsMessageRepository, phoneCallRepository, phoneCallModelFactory, smsMessageModelFactory)
        {
            _contactEntity = contactEntity;
        }

        public override IObservable<IConversationModel> GetItems()
        {
            return
                SmsMessageRepository.GetForContact(_contactEntity)
                    .SelectMany(messages => messages.Select(m => SmsMessageModelFactory.Create(m))).Merge()
                    .Merge(
                        PhoneCallRepository.GetForContact(_contactEntity)
                            .SelectMany(calls => calls.Select(c => PhoneCallModelFactory.Create(c))).Merge());
        }

        protected override IObservable<IConversationModel> GetObservableForOperation(RepositoryMethodEnum method)
        {
            return
                SmsMessageRepository.GetOperationObservable()
                    .OnMethod(method)
                    .ForContact(_contactEntity)
                    .Select(o => SmsMessageModelFactory.Create(o.Item)).Merge()
                    .Merge(
                        PhoneCallRepository.GetOperationObservable()
                            .OnMethod(method)
                            .ForContact(_contactEntity)
                            .Select(o => PhoneCallModelFactory.Create(o.Item)).Merge());
        }
    }
}