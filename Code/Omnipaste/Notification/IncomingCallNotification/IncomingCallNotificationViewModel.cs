namespace Omnipaste.Notification.IncomingCallNotification
{
    using System;
    using Caliburn.Micro;
    using Ninject;
    using OmniApi.Resources.v1;
    using Omnipaste.EventAggregatorMessages;

    public class IncomingCallNotificationViewModel : NotificationViewModelBase, IIncomingCallNotificationViewModel
    {
        private string _endCallButtonText;

        private bool _canEndCall;

        #region Constructors and Destructors

        public IncomingCallNotificationViewModel(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
            EndCallButtonText = "End Call";
            CanEndCall = true;
        }

        #endregion

        #region Public Properties

        public IEventAggregator EventAggregator { get; set; }

        public string PhoneNumber { get; set; }

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
                NotifyOfPropertyChange(() => CanEndCall);
            }
        }

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
                NotifyOfPropertyChange(() => EndCallButtonText);
            }
        }

        [Inject]
        public IDevices Devices { get; set; }

        public override string Title
        {
            get
            {
                return string.Concat("Incoming call from ", PhoneNumber);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void EndCall()
        {
            CanEndCall = false;

            Devices.EndCall()
                .Subscribe(
                    p =>
                    {
                        EndCallButtonText = "Call ended";
                        Dismiss();
                    }, 
                exception => { });
        }

        public void ReplyWithSms()
        {
            EventAggregator.PublishOnUIThread(new SendSmsMessage { Recipient = PhoneNumber, Message = "" });
            Dismiss();
        }

        #endregion
    }
}