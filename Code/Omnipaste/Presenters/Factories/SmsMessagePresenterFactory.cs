namespace Omnipaste.Presenters.Factories
{
    using System;
    using System.Reactive.Linq;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;

    public interface ISmsMessagePresenterFactory
    {
        IObservable<IConversationPresenter> Create(SmsMessageEntity smsMessageEntity);
    }

    public class SmsMessagePresenterFactory : ConversationPresenterFactory, ISmsMessagePresenterFactory
    {
        public SmsMessagePresenterFactory(IContactRepository contactRepository)
            : base(contactRepository)
        {
        }

        #region ISmsMessagePresenterFactory Members

        public IObservable<IConversationPresenter> Create(SmsMessageEntity smsMessageEntity)
        {
            return ContactRepository.Get(smsMessageEntity.ContactInfoUniqueId).Select(
                ci =>
                    {
                        var localSmsMessage = smsMessageEntity as LocalSmsMessageEntity;
                        var smsMessagePresenter = localSmsMessage != null
                            ? (SmsMessagePresenter)new LocalSmsMessagePresenter(localSmsMessage)
                            : new RemoteSmsMessagePresenter((RemoteSmsMessageEntity)smsMessageEntity);
                        smsMessagePresenter.ContactInfoPresenter = new ContactInfoPresenter(ci);

                        return smsMessagePresenter;
                    });
        }

        #endregion
    }
}