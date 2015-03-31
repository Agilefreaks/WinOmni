namespace Omnipaste.Services.Providers
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using Omnipaste.Models;
    using Omnipaste.Models.Factories;
    using Omnipaste.Services.Repositories;

    public abstract class ConversationContext : IConversationContext
    {
        protected readonly IPhoneCallRepository PhoneCallRepository;

        protected readonly IPhoneCallPresenterFactory PhoneCallPresenterFactory;

        protected readonly ISmsMessagePresenterFactory SMSMessagePresenterFactory;

        protected readonly ISmsMessageRepository SmsMessageRepository;

        private IObservable<IConversationModel> _itemChanged;

        private IObservable<IConversationModel> _itemRemoved;

        private IObservable<IConversationModel> _updated;

        protected ConversationContext(
            ISmsMessageRepository smsMessageRepository,
            IPhoneCallRepository phoneCallRepository,
            IPhoneCallPresenterFactory phoneCallPresenterFactory,
            ISmsMessagePresenterFactory smsMessagePresenterFactory)
        {
            SmsMessageRepository = smsMessageRepository;
            PhoneCallRepository = phoneCallRepository;
            PhoneCallPresenterFactory = phoneCallPresenterFactory;
            SMSMessagePresenterFactory = smsMessagePresenterFactory;
        }

        #region IConversationContext Members

        public IObservable<IConversationModel> ItemChanged
        {
            get
            {
                return _itemChanged ?? (_itemChanged = GetObservableForOperation(RepositoryMethodEnum.Changed));
            }
        }

        public IObservable<IConversationModel> ItemRemoved
        {
            get
            {
                return _itemRemoved ?? (_itemRemoved = GetObservableForOperation(RepositoryMethodEnum.Delete));
            }
        }

        public IObservable<IConversationModel> Updated
        {
            get
            {
                return _updated ?? (_updated = ItemChanged.Merge(ItemRemoved));
            }
        }

        public abstract IObservable<IConversationModel> GetItems();

        public virtual IObservable<Unit> SaveItem(IConversationModel item)
        {
            var call = item as PhoneCallModel;
            var result = Observable.Return(new Unit());
            if (call != null)
            {
                result = PhoneCallRepository.Save(call.BackingEntity).Select(_ => new Unit());
            }
            else
            {
                var message = item as SmsMessageModel;
                if (message != null)
                {
                    result = SmsMessageRepository.Save(message.BackingEntity).Select(_ => new Unit());
                }
            }

            return result;
        }

        #endregion

        protected abstract IObservable<IConversationModel> GetObservableForOperation(RepositoryMethodEnum method);
    }
}