namespace Omnipaste.Models.Factories
{
    using System;
    using System.Reactive.Linq;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;

    public interface ISmsMessagePresenterFactory
    {
        IObservable<IConversationModel> Create(SmsMessageEntity smsMessageEntity);
    }

    public class SmsMessageModelFactory : ConversationModelFactory, ISmsMessagePresenterFactory
    {
        public SmsMessageModelFactory(IContactRepository contactRepository)
            : base(contactRepository)
        {
        }

        #region ISmsMessagePresenterFactory Members

        public IObservable<IConversationModel> Create(SmsMessageEntity smsMessageEntity)
        {
            return ContactRepository.Get(smsMessageEntity.ContactInfoUniqueId).Select(
                ci =>
                    {
                        var localSmsMessage = smsMessageEntity as LocalSmsMessageEntity;
                        var smsMessagePresenter = localSmsMessage != null
                            ? (SmsMessageModel)new LocalSmsMessageModel(localSmsMessage)
                            : new RemoteSmsMessageModel((RemoteSmsMessageEntity)smsMessageEntity);
                        smsMessagePresenter.ContactModel = new ContactModel(ci);

                        return smsMessagePresenter;
                    });
        }

        #endregion
    }
}