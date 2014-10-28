namespace OmniDebug.DebugBar.PhoneNotification
{
    using System;
    using Caliburn.Micro;
    using Events.Models;
    using OmniCommon.Models;
    using OmniDebug.Services;

    public class PhoneNotificationViewModel : PropertyChangedBase, IDebugBarPanel
    {
        #region Fields

        private readonly IEventsWrapper _eventsWrapper;

        private readonly IOmniServiceWrapper _omniServiceWrapper;

        private string _notificationContent;

        private string _notificationPhoneNumber;

        private DateTime _notificationTime;

        #endregion

        #region Constructors and Destructors

        public PhoneNotificationViewModel(IOmniServiceWrapper omniServiceWrapper, IEventsWrapper eventsWrapper)
        {
            _omniServiceWrapper = omniServiceWrapper;
            _eventsWrapper = eventsWrapper;
            NotificationTime = DateTime.Now;
        }

        #endregion

        #region Public Properties

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

        #endregion

        #region Public Methods and Operators

        public void SimulateCallNotification()
        {
            _eventsWrapper.MockLast(
                new Event
                    {
                        Content = NotificationContent,
                        Time = NotificationTime,
                        Type = EventTypeEnum.IncomingCallEvent,
                        PhoneNumber = NotificationPhoneNumber
                    });
            _omniServiceWrapper.SimulateMessage(new OmniMessage(OmniMessageTypeEnum.Notification));
        }

        #endregion
    }
}