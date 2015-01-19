namespace OmniDebug.DebugBar.SMSNotification
{
    using System;
    using System.Collections.Generic;
    using Caliburn.Micro;
    using OmniCommon.Models;
    using OmniDebug.Services;
    using SMS.Models;

    public class SMSNotificationViewModel : PropertyChangedBase, IDebugBarPanel
    {
        private readonly IOmniServiceWrapper _omniServiceWrapper;

        private readonly ISmsMessagesWrapper _smsMessagesWrapper;

        private string _notificationContactName;

        private string _notificationContent;

        private string _notificationPhoneNumber;

        private DateTime _notificationTime;

        public SMSNotificationViewModel(IOmniServiceWrapper omniServiceWrapper, ISmsMessagesWrapper smsMessagesWrapper)
        {
            _omniServiceWrapper = omniServiceWrapper;
            _smsMessagesWrapper = smsMessagesWrapper;

            NotificationTime = DateTime.Now;
            NotificationContactName = "Some Contact";
            NotificationContent = "some content";
            NotificationPhoneNumber = "0788999666";
        }

        public string NotificationContactName
        {
            get
            {
                return _notificationContactName;
            }
            set
            {
                if (value == _notificationContactName)
                {
                    return;
                }
                _notificationContactName = value;
                NotifyOfPropertyChange();
            }
        }

        public string NotificationContent
        {
            get
            {
                return _notificationContent;
            }
            set
            {
                if (value == _notificationContent)
                {
                    return;
                }
                _notificationContent = value;
                NotifyOfPropertyChange();
            }
        }

        public string NotificationPhoneNumber
        {
            get
            {
                return _notificationPhoneNumber;
            }
            set
            {
                if (value == _notificationPhoneNumber)
                {
                    return;
                }
                _notificationPhoneNumber = value;
                NotifyOfPropertyChange();
            }
        }

        public DateTime NotificationTime
        {
            get
            {
                return _notificationTime;
            }
            set
            {
                if (value == _notificationTime)
                {
                    return;
                }
                _notificationTime = value;
                NotifyOfPropertyChange();
            }
        }

        public void SimulateSms()
        {
            var id = Guid.NewGuid().ToString();
            _smsMessagesWrapper.MockGet(
                id,
                new SmsMessage
                    {
                        Id = id,
                        Time = NotificationTime,
                        Content = NotificationContent,
                        ContactName = NotificationContactName,
                        PhoneNumber = NotificationPhoneNumber,
                        State = SmsMessageState.Received,
                        Type = SmsMessageType.Incoming
                    });
            _omniServiceWrapper.SimulateMessage(
                new OmniMessage
                    {
                        Type = "sms_message_received",
                        Payload = new Dictionary<string, string> { { "id", id } }
                    });
        }
    }
}