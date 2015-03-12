namespace Omnipaste.Presenters.Factories
{
    using System;
    using System.Reactive.Linq;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;

    public interface ISmsMessagePresenterFactory
    {
        IObservable<IConversationPresenter> Create(SmsMessage smsMessage);
    }

    public class SmsMessagePresenterFactory : ConversationPresenterFactory, ISmsMessagePresenterFactory
    {
        public SmsMessagePresenterFactory(IContactRepository contactRepository)
            : base(contactRepository)
        {
        }

        #region ISmsMessagePresenterFactory Members

        public IObservable<IConversationPresenter> Create(SmsMessage smsMessage)
        {
            return ContactRepository.Get(smsMessage.ContactInfoUniqueId).Select(
                ci =>
                    {
                        var localSmsMessage = smsMessage as LocalSmsMessage;
                        var smsMessagePresenter = localSmsMessage != null
                            ? (SmsMessagePresenter)new LocalSmsMessagePresenter(localSmsMessage)
                            : new RemoteSmsMessagePresenter((RemoteSmsMessage)smsMessage);
                        smsMessagePresenter.ContactInfoPresenter = new ContactInfoPresenter(ci);

                        return smsMessagePresenter;
                    });
        }

        #endregion
    }
}