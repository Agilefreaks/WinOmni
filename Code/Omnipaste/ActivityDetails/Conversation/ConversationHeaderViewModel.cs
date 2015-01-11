namespace Omnipaste.ActivityDetails.Conversation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using Castle.Core.Internal;
    using Ninject;
    using OmniApi.Resources.v1;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Repositories;
    using OmniUI.Presenters;

    public class ConversationHeaderViewModel : ActivityDetailsHeaderViewModel, IConversationHeaderViewModel
    {
        #region Fields

        protected static TimeSpan DelayCallDuration = TimeSpan.FromSeconds(2);

        protected static TimeSpan CallingDuration = TimeSpan.FromSeconds(5);

        private IContactInfoPresenter _contactInfo;

        private ConversationHeaderStateEnum _state;

        private IDisposable _callSubscription;

        private IList<Models.Message> _deletedMessages;

        private List<Models.Call> _deletedCalls;

        #endregion

        #region Public Properties

        public IContactInfoPresenter ContactInfo
        {
            get
            {
                return _contactInfo;
            }
            set
            {
                if (Equals(value, _contactInfo))
                {
                    return;
                }
                _contactInfo = value;
                NotifyOfPropertyChange();
            }
        }

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
                NotifyOfPropertyChange(() => State);
            }
        }

        public override ActivityPresenter Model
        {
            get
            {
                return base.Model;
            }
            set
            {
                base.Model = value;
                ContactInfo = new ContactInfoPresenter(value.ExtraData.ContactInfo);
            }
        }

        [Inject]
        public IDevices Devices { get; set; }

        [Inject]
        public ICallRepository CallRepository { get; set; }

        [Inject]
        public IMessageRepository MessageRepository { get; set; }

        #endregion

        public void Call()
        {
            DisposeCallSubscription();
            _callSubscription = Observable.Interval(DelayCallDuration, SchedulerProvider.Default)
                .Take(1, SchedulerProvider.Default)
                .Do(_ => { State = ConversationHeaderStateEnum.Calling; })
                .Select(_ => Devices.Call(Model.BackingModel.ExtraData.ContactInfo.Phone as string))
                .Switch()
                .Delay(CallingDuration, SchedulerProvider.Default)
                .Do(_ =>  { State = ConversationHeaderStateEnum.Normal; })
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
            CallRepository.GetByContact(ContactInfo.ContactInfo)
                .SubscribeOn(SchedulerProvider.Default)
                .ObserveOn(SchedulerProvider.Dispatcher)
                .SubscribeAndHandleErrors(DeleteCalls);
            MessageRepository.GetByContact(ContactInfo.ContactInfo)
                .SubscribeOn(SchedulerProvider.Default)
                .ObserveOn(SchedulerProvider.Dispatcher)
                .SubscribeAndHandleErrors(DeleteMessages);
            State = ConversationHeaderStateEnum.Deleted;
        }

        public void UndoDelete()
        {
            RestoreCalls();
            RestoreMessages();
            State = ConversationHeaderStateEnum.Normal;
        }

        protected override void OnActivate()
        {
            State = ConversationHeaderStateEnum.Normal;
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            _deletedCalls = null;
            _deletedMessages = null;
            base.OnDeactivate(close);
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

        private void DeleteCalls(IEnumerable<Models.Call> calls)
        {
            _deletedCalls = calls.ToList();
            _deletedCalls.ForEach(call => CallRepository.Delete(call.UniqueId));
        }

        private void DeleteMessages(IEnumerable<Models.Message> messages)
        {
            _deletedMessages = messages.ToList();
            _deletedMessages.ForEach(message => MessageRepository.Delete(message.UniqueId));
        }

        private void RestoreCalls()
        {
            if (_deletedCalls == null)
            {
                return;
            }

            _deletedCalls.ForEach(call => CallRepository.Save(call));
            _deletedCalls = null;
        }

        private void RestoreMessages()
        {
            if (_deletedMessages == null)
            {
                return;
            }

            _deletedMessages.ForEach(message => MessageRepository.Save(message));
            _deletedMessages = null;
        }
    }
}