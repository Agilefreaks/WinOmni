namespace Omnipaste.Notification.IncomingCallNotification
{
    using System;
    using Caliburn.Micro;
    using Ninject;
    using OmniApi.Resources.v1;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Properties;

    public class IncomingCallNotificationViewModel : NotificationViewModelBase, IIncomingCallNotificationViewModel
    {
        private string _endCallButtonText;

        private bool _canEndCall;

        private bool _canReplyWithSms = true;

        #region Constructors and Destructors

        public IncomingCallNotificationViewModel(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
            EndCallButtonText = Resources.IncommingCallNotificationEndCall;
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

        public bool CanReplyWithSms
        {
            get
            {
                return _canReplyWithSms;
            }
            set
            {
                if (value.Equals(_canReplyWithSms))
                {
                    return;
                }
                _canReplyWithSms = value;
                NotifyOfPropertyChange(() => CanReplyWithSms);
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
                return string.Concat(Resources.IncommingCallNotificationTitle, PhoneNumber);
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
                        EndCallButtonText = Resources.IncommingCallNotificationCallEnded;
                        Dismiss();
                    }, 
                exception => { });
        }

        public void ReplyWithSms()
        {
            CanReplyWithSms = false;
            EventAggregator.PublishOnUIThread(new SendSmsMessage { Recipient = PhoneNumber, Message = "" });
            Dismiss();
        }

        #endregion
    }
}