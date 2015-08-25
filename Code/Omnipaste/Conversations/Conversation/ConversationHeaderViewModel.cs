namespace Omnipaste.Conversations.Conversation
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reactive.Linq;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Entities.Factories;
    using Omnipaste.Framework.Models;
    using Omnipaste.Framework.Services.Repositories;
    using OmniUI.Details;
    using PhoneCalls.Resources.v1;

    public class ConversationHeaderViewModel : DetailsViewModelBase<ContactModel>, IConversationHeaderViewModel
    {
        protected static TimeSpan CallingDuration = TimeSpan.FromSeconds(5);

        protected static TimeSpan DelayCallDuration = TimeSpan.FromSeconds(2);

        private IDisposable _callSubscription;

        private ObservableCollection<ContactModel> _recipients;

        private ConversationHeaderStateEnum _state;

        private string _tokenizedRecipients;

        private IRecepientsTokenizer _recepientsTokenizer;

        [Inject]
        public IPhoneCallFactory PhoneCallFactory { get; set; }

        [Inject]
        public IPhoneCallRepository PhoneCallRepository { get; set; }

        [Inject]
        public ISmsMessageRepository SmsMessageRepository { get; set; }

        [Inject]
        public IPhoneCalls PhoneCalls { get; set; }

        public IRecepientsTokenizer RecepientsTokenizer
        {
            get
            {
                return _recepientsTokenizer;
            }
            set
            {
                if (_recepientsTokenizer != null)
                {
                    return;
                }
                _recepientsTokenizer = value;
            }
        }

        public TimeSpan ProgressDuration
        {
            get
            {
                return DelayCallDuration;
            }
        }

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
            UpdateConversationItems(PhoneCallRepository, true);
            UpdateConversationItems(SmsMessageRepository, true);
            State = ConversationHeaderStateEnum.Deleted;
        }

        public void UndoDelete()
        {
            UpdateConversationItems(PhoneCallRepository, false);
            UpdateConversationItems(SmsMessageRepository, false);
            State = ConversationHeaderStateEnum.Normal;
        }

        protected override void OnActivate()
        {
            Recipients = Recipients ?? new ObservableCollection<ContactModel>();
            Recipients.CollectionChanged += RecipientsOnCollectionChanged;

            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            DeleteConversationItems<PhoneCallEntity>(PhoneCallRepository);
            DeleteConversationItems<SmsMessageEntity>(SmsMessageRepository);

            base.OnDeactivate(close);
        }

        private void RecipientsOnCollectionChanged(
            object sender,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (State == ConversationHeaderStateEnum.Edit)
            {
                TokenizedRecipients = RecepientsTokenizer.Tokenize();
                return;
            }

            if (_recipients.Count >= 2)
            {
                State = ConversationHeaderStateEnum.Group;
            }
            else if (_recipients.Count == 0)
            {
                State = ConversationHeaderStateEnum.Edit;
            }
            else if (State == ConversationHeaderStateEnum.Group)
            {
                State = ConversationHeaderStateEnum.Normal;
            }
        }

        private void DeleteConversationItems<T>(IConversationRepository repository) where T : ConversationEntity
        {
            if (Model == null)
            {
                return;
            }

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

        private void UpdateConversationItems(IConversationRepository repository, bool isDeleted)
        {
            if (Model == null)
            {
                return;
            }

            repository.GetConversationForContact(Model.BackingEntity).SelectMany(
                items => items.Select(
                    item =>
                        {
                            item.IsDeleted = isDeleted;
                            return repository.Save(item);
                        })).Switch().SubscribeAndHandleErrors();
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
                RecepientsTokenizer = new RecepientsTokenizer(_recipients);
                RecipientsOnCollectionChanged(
                    _recipients,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                NotifyOfPropertyChange();
            }
        }

        public string TokenizedRecipients
        {
            get
            {
                return _tokenizedRecipients;
            }
            set
            {
                _tokenizedRecipients = value;
                NotifyOfPropertyChange();
            }
        }

        #endregion
    }
}