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

    public class ConversationHeaderViewModel : WorkspaceDetailsHeaderViewModel<ContactInfoPresenter>, IConversationHeaderViewModel
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
        public IMessageRepository MessageRepository { get; set; }

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
                    .Select(_ => PhoneCalls.Call(Model.ContactInfo.PhoneNumber, Model.ContactInfo.ContactId))
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
            UpdateConversationItems(PhoneCallRepository, item => item.IsDeleted = true);
            UpdateConversationItems(MessageRepository, item => item.IsDeleted = true);
            State = ConversationHeaderStateEnum.Deleted;
        }

        public void UndoDelete()
        {
            UpdateConversationItems(PhoneCallRepository, item => item.IsDeleted = false);
            UpdateConversationItems(MessageRepository, item => item.IsDeleted = false);
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
            DeleteConversationItems(PhoneCallRepository);
            DeleteConversationItems(MessageRepository);
            base.OnDeactivate(close);
        }

        private void DeleteConversationItems<T>(IRepository<T> repository) where T : BaseModel, IConversationItem
        {
            repository.GetByContact(Model.ContactInfo)
                .SubscribeAndHandleErrors(
                    items =>
                    items.Where(c => c.IsDeleted).ToList().ForEach(c => repository.Delete(c.UniqueId).RunToCompletion()));
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

        private void UpdateConversationItems<T>(IRepository<T> repository, Action<T> update)
            where T : BaseModel, IConversationItem
        {
            repository.GetByContact(Model.ContactInfo).SelectMany(
                items => items.Select(
                    i =>
                        {
                            update(i);
                            return repository.Save(i);
                        }))
                        .Switch()
                        .SubscribeAndHandleErrors();
        }

        #endregion
    }
}