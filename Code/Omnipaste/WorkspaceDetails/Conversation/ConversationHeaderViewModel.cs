namespace Omnipaste.WorkspaceDetails.Conversation
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.Factories;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Repositories;
    using PhoneCalls.Resources.v1;

    public class ConversationHeaderViewModel : WorkspaceDetailsHeaderViewModel<ContactInfoPresenter>,
                                               IConversationHeaderViewModel
    {
        #region Static Fields

        protected static TimeSpan CallingDuration = TimeSpan.FromSeconds(5);

        protected static TimeSpan DelayCallDuration = TimeSpan.FromSeconds(2);

        #endregion

        #region Fields

        private IDisposable _callSubscription;

        private ConversationHeaderStateEnum _state;

        #endregion

        #region Public Properties

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

        #endregion

        #region Public Methods and Operators

        public void Call()
        {
            DisposeCallSubscription();
            _callSubscription =
                Observable.Timer(DelayCallDuration, SchedulerProvider.Default)
                    .Do(_ => State = ConversationHeaderStateEnum.Calling)
                    .Select(_ => PhoneCalls.Call(Model.BackingModel.PhoneNumber, Model.BackingModel.ContactId))
                    .Switch()
                    .Select(phoneCallDto => PhoneCallFactory.Create<LocalPhoneCall>(phoneCallDto))
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
            UpdateConversationItems<Models.PhoneCall>(PhoneCallRepository, true);
            UpdateConversationItems<SmsMessage>(SmsMessageRepository, true);
            State = ConversationHeaderStateEnum.Deleted;
        }

        public void UndoDelete()
        {
            UpdateConversationItems<Models.PhoneCall>(PhoneCallRepository, false);
            UpdateConversationItems<SmsMessage>(SmsMessageRepository, false);
            State = ConversationHeaderStateEnum.Normal;
        }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            State = ConversationHeaderStateEnum.Normal;
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            DeleteConversationItems<Models.PhoneCall>(PhoneCallRepository);
            DeleteConversationItems<SmsMessage>(SmsMessageRepository);
            base.OnDeactivate(close);
        }

        private void DeleteConversationItems<T>(IConversationRepository repository) where T : ConversationBaseModel
        {
            repository.GetConversationForContact(Model.BackingModel)
                .SubscribeAndHandleErrors(
                    items =>
                    items.Where(c => c.IsDeleted).ToList().ForEach(c => repository.Delete<T>(c.UniqueId).RunToCompletion()));
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
            where T : ConversationBaseModel
        {
            repository.GetConversationForContact(Model.BackingModel).SelectMany(
                items => items.Select(
                    item =>
                        {
                            item.IsDeleted = isDeleted;
                            return repository.Save(item);
                        })).Switch().SubscribeAndHandleErrors();
        }

        #endregion
    }
}