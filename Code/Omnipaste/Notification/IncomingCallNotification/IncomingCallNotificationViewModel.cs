namespace Omnipaste.Notification.IncomingCallNotification
{
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.Properties;
    using PhoneCalls.Resources.v1;

    public class IncomingCallNotificationViewModel : ConversationNotificationViewModelBase, IIncomingCallNotificationViewModel
    {
        #region Fields

        private bool _canEndCall;

        private string _endCallButtonText;

        #endregion

        #region Constructors and Destructors

        public IncomingCallNotificationViewModel(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
            EndCallButtonText = Resources.IncommingCallNotificationEndCall;
            CanEndCall = true;
        }

        #endregion

        #region Public Properties

        public bool CanEndCall
        {
            get
            {
                return _canEndCall;
            }
            set
            {
                if (value.Equals(_canEndCall))
                {
                    return;
                }
                _canEndCall = value;
                NotifyOfPropertyChange();
            }
        }

        [Inject]
        public IPhoneCalls PhoneCalls { get; set; }

        public string EndCallButtonText
        {
            get
            {
                return _endCallButtonText;
            }
            set
            {
                if (value == _endCallButtonText)
                {
                    return;
                }
                _endCallButtonText = value;
                NotifyOfPropertyChange();
            }
        }

        public override string Title
        {
            get
            {
                return Resources.IncommingCallNotificationTitle;
            }
        }

        public override NotificationTypeEnum Type
        {
            get
            {
                return NotificationTypeEnum.IncomingCall;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void EndCall()
        {
            CanEndCall = false;

            PhoneCalls.EndCall(Resource.Id).RunToCompletion(
                _ =>
                    {
                        EndCallButtonText = Resources.IncommingCallNotificationCallEnded;
                        Dismiss();
                    });
        }

        #endregion
    }
}