namespace Omnipaste.ActivityDetails.Conversation
{
    using System;
    using System.Reactive.Linq;
    using Ninject;
    using OmniApi.Resources.v1;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.Presenters;
    using OmniUI.Presenters;

    public class ConversationHeaderViewModel : ActivityDetailsHeaderViewModel, IConversationHeaderViewModel
    {
        #region Fields

        protected static TimeSpan DelayCallDuration = TimeSpan.FromSeconds(2);

        protected static TimeSpan CallingDuration = TimeSpan.FromSeconds(5);

        private IContactInfoPresenter _contactInfo;

        private ConversationHeaderStateEnum _state;

        private IDisposable _callSubscription;

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
            State = ConversationHeaderStateEnum.Deleted;
        }

        public void UndoDelete()
        {
            State = ConversationHeaderStateEnum.Normal;
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
    }
}