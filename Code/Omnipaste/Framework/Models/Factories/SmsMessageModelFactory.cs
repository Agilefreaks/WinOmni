namespace Omnipaste.Framework.Models.Factories
{
    using System;
    using System.Reactive.Linq;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Services.Repositories;

    public interface ISmsMessageModelFactory
    {
        IObservable<IConversationModel> Create(SmsMessageEntity smsMessageEntity);
    }

    public class SmsMessageModelFactory : ConversationModelFactory, ISmsMessageModelFactory
    {
        public SmsMessageModelFactory(IContactRepository contactRepository)
            : base(contactRepository)
        {
        }

        #region ISmsMessageModelFactory Members

        public IObservable<IConversationModel> Create(SmsMessageEntity smsMessageEntity)
        {
            return ContactRepository.Get(smsMessageEntity.ContactUniqueId).Select(
                ci =>
                    {
                        var localSmsMessage = smsMessageEntity as LocalSmsMessageEntity;
                        var messageModel = localSmsMessage != null
                            ? (SmsMessageModel)new LocalSmsMessageModel(localSmsMessage)
                            : new RemoteSmsMessageModel((RemoteSmsMessageEntity)smsMessageEntity);
                        messageModel.ContactModel = new ContactModel(ci);

                        return messageModel;
                    });
        }

        #endregion
    }
}