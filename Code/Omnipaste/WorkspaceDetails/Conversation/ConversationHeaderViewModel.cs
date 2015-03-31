namespace Omnipaste.WorkspaceDetails.Conversation
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reactive.Linq;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.Entities;
    using Omnipaste.Factories;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using PhoneCalls.Resources.v1;

    public class ConversationHeaderViewModel : WorkspaceDetailsHeaderViewModel<ContactModel>,
                                               IConversationHeaderViewModel
    {
        protected static TimeSpan CallingDuration = TimeSpan.FromSeconds(5);

        protected static TimeSpan DelayCallDuration = TimeSpan.FromSeconds(2);

        private IDisposable _callSubscription;

        private ObservableCollection<ContactModel> _recipients;

        private ConversationHeaderStateEnum _state;

        [Inject]
        public IPhoneCallFactory PhoneCallFactory { get; set; }

        [Inject]
        public IPhoneCallRepository PhoneCallRepository { get; set; }

        [Inject]
        public ISmsMessageRepository SmsMessageRepository { get; set; }

        [Inject]
        public IPhoneCalls PhoneCalls { get; set; }

        public TimeSpan ProgressDuration
        {
            get
            {
                return DelayCallDuration;
            }
        }

        #region IConversationHeaderViewModel Members

        public ConversationHeaderStateEnum State
        {
            get
            {
                return _state;
            }
            set
            {
                if (value == _state)
                {
                    return;
                }
                _state = value;
                NotifyOfPropertyChange();
            }
        }

        public ObservableCollection<ContactModel> Recipients
        {
            get
            {
                return _recipients;
            }
            set
            {
                if (Equals(value, _recipients))
                {
                    return;
                }

                _recipients = value;
                RecipientsOnCollectionChanged(_recipients, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                Recipients.CollectionChanged += RecipientsOnCollectionChanged;
                NotifyOfPropertyChange(() => Recipients);
            }
        }

        #endregion

        public void Call()
        {
            DisposeCallSubscription();
            _callSubscription =
                Observable.Timer(DelayCallDuration, SchedulerProvider.Default)
                    .Do(_ => State = ConversationHeaderStateEnum.Calling)
                    .Select(_ => PhoneCalls.Call(Model.BackingEntity.PhoneNumber, Model.BackingEntity.ContactId))
                    .Switch()
                    .Select(phoneCallDto => PhoneCallFactory.Create<LocalPhoneCallEntity>(phoneCallDto))
                    .Switch()
                    .Delay(CallingDuration, SchedulerProvider.Default)
                    .Do(_ => State = ConversationHeaderStateEnum.Normal)
                    .SubscribeAndHandleErrors();

            State = ConversationHeaderStateEnum.InitiatingCall;
        }

        public void CancelCall()
        {
            DisposeCallSubscription();

            State = ConversationHeaderStateEnum.Normal;
        }

        public void Delete()
        {
            UpdateConversationItems<Entities.PhoneCallEntity>(PhoneCallRepository, true);
            UpdateConversationItems<SmsMessageEntity>(SmsMessageRepository, true);
            State = ConversationHeaderStateEnum.Deleted;
        }

        public void UndoDelete()
        {
            UpdateConversationItems<Entities.PhoneCallEntity>(PhoneCallRepository, false);
            UpdateConversationItems<SmsMessageEntity>(SmsMessageRepository, false);
            State = ConversationHeaderStateEnum.Normal;
        }

        protected override void OnActivate()
        {
            State = ConversationHeaderStateEnum.Normal;
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            DeleteConversationItems<Entities.PhoneCallEntity>(PhoneCallRepository);
            DeleteConversationItems<SmsMessageEntity>(SmsMessageRepository);

            if (Recipients != null)
            {
                Recipients.CollectionChanged -= RecipientsOnCollectionChanged;
            }

            base.OnDeactivate(close);
        }

        private void RecipientsOnCollectionChanged(
            object sender,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            State = _recipients.Count >= 2 ? ConversationHeaderStateEnum.Group : State;
        }

        private void DeleteConversationItems<T>(IConversationRepository repository) where T : ConversationEntity
        {
            repository.GetConversationForContact(Model.BackingEntity)
                .SubscribeAndHandleErrors(
                    items =>
                    items.Where(c => c.IsDeleted)
                        .ToList()
                        .ForEach(c => repository.Delete<T>(c.UniqueId).RunToCompletion()));
        }

        private void DisposeCallSubscription()
        {
            if (_callSubscription == null)
            {
                return;
            }

            _callSubscription.Dispose();
            _callSubscription = null;
        }

        private void UpdateConversationItems<T>(IConversationRepository repository, bool isDeleted)
            where T : ConversationEntity
        {
            repository.GetConversationForContact(Model.BackingEntity).SelectMany(
                items => items.Select(
                    item =>
                        {
                            item.IsDeleted = isDeleted;
                            return repository.Save(item);
                        })).Switch().SubscribeAndHandleErrors();
        }
    }
}