namespace Omnipaste.Services.Providers
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using Omnipaste.Presenters;
    using Omnipaste.Presenters.Factories;
    using Omnipaste.Services.Repositories;

    public abstract class ConversationContext : IConversationContext
    {
        protected readonly IPhoneCallRepository PhoneCallRepository;

        protected readonly IPhoneCallPresenterFactory PhoneCallPresenterFactory;

        protected readonly ISmsMessagePresenterFactory SMSMessagePresenterFactory;

        protected readonly ISmsMessageRepository SmsMessageRepository;

        private IObservable<IConversationPresenter> _itemChanged;

        private IObservable<IConversationPresenter> _itemRemoved;

        private IObservable<IConversationPresenter> _updated;

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

        public IObservable<IConversationPresenter> ItemChanged
        {
            get
            {
                return _itemChanged ?? (_itemChanged = GetObservableForOperation(RepositoryMethodEnum.Changed));
            }
        }

        public IObservable<IConversationPresenter> ItemRemoved
        {
            get
            {
                return _itemRemoved ?? (_itemRemoved = GetObservableForOperation(RepositoryMethodEnum.Delete));
            }
        }

        public IObservable<IConversationPresenter> Updated
        {
            get
            {
                return _updated ?? (_updated = ItemChanged.Merge(ItemRemoved));
            }
        }

        public abstract IObservable<IConversationPresenter> GetItems();

        public virtual IObservable<Unit> SaveItem(IConversationPresenter item)
        {
            var call = item as PhoneCallPresenter;
            var result = Observable.Return(new Unit());
            if (call != null)
            {
                result = PhoneCallRepository.Save(call.BackingModel).Select(_ => new Unit());
            }
            else
            {
                var message = item as SmsMessagePresenter;
                if (message != null)
                {
                    result = SmsMessageRepository.Save(message.BackingModel).Select(_ => new Unit());
                }
            }

            return result;
        }

        #endregion

        protected abstract IObservable<IConversationPresenter> GetObservableForOperation(RepositoryMethodEnum method);
    }
}